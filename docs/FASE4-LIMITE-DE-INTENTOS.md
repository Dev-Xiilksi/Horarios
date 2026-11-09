# Fase 4: límite de intentos de inicio de sesión

Qué se hizo, cómo funciona y qué falta de la Fase 4.

## El problema

El login no tenía límite de intentos. Eso permitía dos ataques:

- **Adivinar la contraseña de una cuenta** probando muchas seguidas. Como cada intento llega a Active Directory, también se podía **bloquear la cuenta en el dominio** a propósito y dejar a la persona sin acceso a otros servicios.
- **Probar una contraseña común contra muchos usuarios** desde la misma IP.

## Por qué no bastaba con el middleware de ASP.NET Core

ASP.NET Core trae `AddRateLimiter`, pero solo limita **peticiones HTTP**. En esta app, el usuario y la contraseña se envían desde un componente Blazor interactivo, por la conexión SignalR del circuito, y no en una petición HTTP propia. El middleware nunca vería esos intentos.

Por eso el límite está **dentro del caso de uso de login** (`LoginService`), que es por donde pasa todo intento, venga de donde venga.

## Reglas

| Límite | Valor | Para qué |
|---|---|---|
| Intentos fallidos por usuario | 5 cada 15 minutos | Impide adivinar la contraseña de una cuenta y bloquearla en el AD |
| Intentos fallidos por IP | 20 cada 15 minutos | Impide probar contraseñas contra muchos usuarios |
| Peticiones a `/Account/Login` y `/Account/Logout` por IP | 10 por minuto | Protección básica de esos endpoints HTTP |

- **Qué cuenta como intento:** solo cuando Active Directory dice que la contraseña es incorrecta. Un inicio de sesión correcto no gasta intentos, ni tampoco el aviso "No tienes permiso".
- **Por qué no cuentan los correctos:** si muchos docentes entran a la misma hora desde la misma IP (por ejemplo, detrás de un proxy o un NAT), los inicios correctos no deben bloquearse entre sí.
- **Ventana deslizante:** la ventana de 15 minutos está dividida en tres tramos de 5 minutos, y los intentos más antiguos se liberan poco a poco. Esto evita que un atacante espere al cambio de ventana y vuelva a tener todos los intentos de golpe.
- **Variantes del nombre:** `JPerez`, `LICEO\jperez` y `jperez@liceojavier.edu.gt` cuentan como el mismo usuario.
- **Intento bloqueado:** se rechaza **antes de consultar Active Directory**, así que el dominio no recibe esos intentos. El usuario ve "Demasiados intentos de inicio de sesión. Espere 15 minutos e intente de nuevo." y el log registra una advertencia con el usuario y la IP, nunca la contraseña.

## Cómo funciona

```text
Petición HTTP inicial ──> App.razor toma la IP ──> Routes.razor la reparte ──> Login.razor
                                                                                   │
                                     LoginService ──> LimitadorIntentosLogin ◄─────┘
                                          │  ¿quedan intentos? no ──> "Demasiados intentos" (sin ir al AD)
                                          │  sí
                                          ▼
                                   Active Directory ──> contraseña incorrecta ──> RegistrarFallo
```

| Archivo | Qué hace |
|---|---|
| `Application/Seguridad/LimitadorIntentosLogin.cs` | Lleva el conteo por usuario y por IP con `System.Threading.RateLimiting`. `PuedeIntentar` consulta sin gastar intentos y `RegistrarFallo` gasta uno |
| `Application/Services/LoginService.cs` | Consulta el límite antes de ir al AD y registra el fallo si la contraseña es incorrecta |
| `Application/DependencyInjection.cs` | Registra el limitador como **singleton**, para compartir el conteo entre todas las conexiones |
| `SistemaHorarioApp/Components/App.razor` | Toma la IP de la petición HTTP inicial |
| `SistemaHorarioApp/Components/Routes.razor` | La reparte a las páginas como valor en cascada (`IpCliente`) |
| `SistemaHorarioApp/Components/Pages/Login/Login.razor` | Envía la IP a `ILoginService.Login(login, IpCliente)` |
| `SistemaHorarioApp/Program.cs` | Configura `AddRateLimiter` y `UseRateLimiter` para `/Account/Login` y `/Account/Logout` |
| `SistemaHorarioApp/Seguridad/PoliticasLimite.cs` | Nombre de la política HTTP |

### La IP del navegador

Durante el circuito interactivo de Blazor ya no existe `HttpContext`. Por eso la IP se toma en `App.razor` mientras se atiende la primera petición HTTP y se pasa como parámetro al componente interactivo.

El navegador no puede cambiar ese valor: Blazor guarda los parámetros del componente raíz cifrados con Data Protection.

Si la IP no se conoce, solo se aplica el límite por usuario. Así se evita que todos compartan un mismo contador y se bloqueen entre sí.

## Cómo cambiar los límites

Los valores están en `LimitesLogin.PorDefecto`, dentro de `LimitadorIntentosLogin.cs`:

```csharp
public static LimitesLogin PorDefecto => new(5, 20, TimeSpan.FromMinutes(15));
```

El primer valor es el límite por usuario, el segundo el límite por IP y el tercero la duración de la ventana. El mensaje al usuario toma la duración de la ventana automáticamente.

## Limitaciones

- **El conteo vive en memoria.** Se reinicia al reciclar el grupo de aplicaciones de IIS. Con varios servidores, cada uno llevaría su propio conteo.
- **La IP depende de cómo llega el tráfico.** Si hay un proxy o balanceador delante de IIS, la app verá la IP del proxy para todos. Revisa en el log de producción qué IP aparece en `Login bloqueado...`. Si siempre es la misma, hay que configurar `ForwardedHeaders` antes de confiar en el límite por IP.
- **Bloquear la cuenta de alguien desde fuera:** un atacante puede impedir el acceso de un usuario durante 15 minutos con 5 intentos fallidos. Es preferible a que llegue a bloquear la cuenta en el AD, y se puede ajustar con los valores de arriba.

## Qué se comprobó

| Prueba | Resultado |
|---|---|
| Tests automáticos | 31 de 31 pasan (9 nuevos de esta fase) |
| Usuario con contraseña incorrecta en la app | Cuatro intentos devolvieron "Credenciales incorrectas." y del quinto en adelante "Demasiados intentos..."; contando el fallo de una tanda anterior con el mismo usuario, se bloqueó tras cinco fallos |
| Log del bloqueo | Registra usuario e IP (`127.0.0.1` en local), lo que confirma que la IP llega hasta `LoginService` |
| 12 peticiones seguidas a `/Account/Login` | Las primeras 10 responden con normalidad y las 2 siguientes reciben **429** |
| Otra página después de ese bloqueo | 200: el límite HTTP solo afecta a `/Account/*` |

Entre los tests nuevos está **"Los inicios de sesión correctos no gastan intentos"**: 10 docentes distintos entran desde la misma IP con un límite de 2, y ninguno queda bloqueado.

## Qué no se comprobó

- **IP en producción:** no se comprobó qué IP llega en IIS (ver la limitación del proxy).
- **Bloqueo por IP:** el límite por IP con muchos usuarios distintos solo se probó con tests, no en la app.

## Lo que falta de la Fase 4

Esta entrega cubre el límite de intentos. Del plan original de la Fase 4 quedan pendientes:

- [ ] Cabeceras de seguridad (`Content-Security-Policy`, `X-Content-Type-Options`, `Referrer-Policy`, `X-Frame-Options`).
- [ ] Mensajes de error genéricos en pantalla. Hoy algunas pantallas muestran `ex.Message`, y `EliminarHorario` devuelve el mensaje de la excepción.
- [ ] Fotos de empleados accesibles solo con sesión. Hoy están en `wwwroot/employees` y son públicas.
- [ ] Revisar quién puede leer las tablas de registro (`hr_log_errors` y `app_log`).
