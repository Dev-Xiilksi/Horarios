# Seguridad

Estado al 6 de noviembre de 2026: Fases 1 a 3 y el límite de intentos de la Fase 4 aplicados en el código, **sin commit**. La Fase 2 ya está configurada en el servidor. Reglas por rol: ver [Fase 3](FASE3-REGLAS-POR-ROL.md). La Fase 2 requiere pasos en el servidor: ver [Fase 2](FASE2-SECRETOS-Y-CONFIGURACION.md).

## Cómo funciona el inicio de sesión

Los intentos fallidos están limitados: 5 por usuario y 20 por IP cada 15 minutos. Ver [Fase 4](FASE4-LIMITE-DE-INTENTOS.md).

1. `Login.razor` envía usuario y contraseña a `ILoginService`.
2. `LoginService` valida contra Active Directory y comprueba que el usuario tenga acceso registrado o sea maestro. Una sesión solo se completa si existe un registro de acceso.
3. `LoginTicketStore` guarda la sesión validada en memoria del servidor y devuelve un ticket aleatorio de 32 bytes, de un solo uso, que caduca a los 30 segundos.
4. El navegador va a `/Account/Login?ticket=...`.
5. `Login.cshtml.cs` canjea el ticket y crea la cookie. Si el ticket no es válido, vuelve al login sin crear sesión.

Esa página ya no acepta usuario, rol ni área desde la URL.

**Limitación:** el ticket vive en la memoria de un solo proceso. Con varios servidores detrás de un balanceador habría que moverlo a un almacén compartido.

## Cookie de sesión

| Opción | Valor |
|---|---|
| Nombre | `SistemaHorario.Sesion` |
| `HttpOnly` | Sí |
| `SameSite` | `Strict` |
| Duración | 60 minutos, se renueva con el uso |
| `SecurePolicy` | `Always` en producción; `SameAsRequest` en desarrollo. Lo controla `Seguridad:RequiereHttps` |

La sesión guarda estos datos: `idUser`, nombre, `group`, rol, `area` e `idArea`.

Cerrar sesión es un POST con token antifalsificación. Un GET a `/Account/Logout` solo redirige.

## Permisos por página

`Components/Pages/_Imports.razor` y `Dialog/_Imports.razor` exigen sesión en todo su contenido. `Routes.razor` usa `AuthorizeRouteView`, de modo que los permisos también se revisan al navegar dentro de la app.

- Sin sesión: redirige al login.
- Con sesión pero sin el rol: muestra `SinPermiso`.

| Ruta | Acceso |
|---|---|
| `/`, `/Error`, `/unauthorized` | Público |
| `/acceso-usuario` | Administrador |
| `/Educadores/DataProfile/{Empleado}` | Administrador |
| `/page/area` | Administrador, Consultor |
| `/page/obtener-consolidados` | Administrador, Consultor |
| `/page/home`, `/page/horario`, `/page/horario-fijo`, `/page/horario-variable` | Cualquier usuario con sesión |
| `/cons`, `/dash`, `/area/{Codigo}/{id}/{ciclo}`, `/dialogcard/{obj}`, `/ss` | Cualquier usuario con sesión; falta definir el rol |

`/ss` es una versión antigua de la pantalla de horario variable. Falta decidir si se elimina.

## Qué se comprobó

Con la app corriendo en Development y sin sesión:

| Prueba | Resultado |
|---|---|
| `/Account/Login` con rol en la URL | Redirige al login, sin cookie |
| `/Account/Login` con ticket inventado | Redirige al login, sin cookie |
| `/page/home`, `/acceso-usuario`, `/dash` | Redirigen al login |
| `/test`, `/counter` | 404 |
| POST a `/Account/Logout` sin token | 400 |
| Pantalla de login | Carga y responde sin sesión |
| Usuario de prueba con contraseña falsa | "Credenciales incorrectas." |

## Qué falta comprobar

Estas pruebas necesitan usuarios reales:

- [ ] Iniciar sesión con cada rol y llegar al inicio correcto.
- [ ] Como Educador, abrir `/acceso-usuario` y `/page/obtener-consolidados`: debe verse "No tienes permiso".
- [ ] Hacer lo mismo navegando dentro de la app, sin recargar.
- [ ] Cerrar sesión con el botón. El formulario con token no se ha probado con una sesión activa.
- [ ] Reutilizar un enlace `/Account/Login?ticket=...` ya usado: no debe crear sesión.

## Riesgos abiertos

| Prioridad | Riesgo | Fase |
|---|---|---|
| Alta | Las contraseñas actuales siguen en `APP_backup_antes_clean_arch` y en las publicaciones anteriores. Hay que renovarlas | 2, pendiente en servidor |
| Alta | Desarrollo usa la cuenta `sa` y producción un usuario SQL compartido. El script del usuario nuevo está listo, sin ejecutar | 2, pendiente en servidor |
| Media | Varios servicios de solo lectura de `Infrastructure/Queries` no comprueban quién los pide (gráficas por empleado o área, listado de colaboradores, perfil). Ver [Fase 3](FASE3-REGLAS-POR-ROL.md#lo-que-no-cubre-esta-fase) | 3, pendiente |
| Alta | El código ya exige HTTPS, pero falta instalar el certificado y publicar | 2, pendiente en servidor |
| Media | `TrustServerCertificate=true` en la cadena de conexión | 2, pendiente en servidor |
| Media | Las fotos en `wwwroot/employees` son públicas y también están en el historial de git (312 archivos). No se debe subir este repositorio a un remoto sin decidir antes qué hacer con ellas | 4 |
| Media | `EliminarHorario` devuelve el mensaje técnico de la excepción a la pantalla | 4 |

El plan completo está en la [página del plan](https://claude.ai/artifact/BEvvAwcVEbaD5ozDqReGhK).
