# Fase 2: secretos y configuración

Qué se hizo, cómo funciona y qué hay que hacer en el servidor.

**Antes de publicar esta versión:** configura el servidor siguiendo los [pasos de abajo](#pasos-en-el-servidor). Sin esos valores, la app no arranca, a propósito.

## Resumen

| Tema | Antes | Ahora |
|---|---|---|
| Contraseña SQL y clave JWT | Dentro de `appsettings*.json` | Fuera de los archivos del proyecto |
| Valores en desarrollo | `appsettings.Development.json` | `dotnet user-secrets` |
| Valores en producción | `appsettings.json` | Variables de entorno |
| Si falta un secreto | Fallaba más tarde, al usarlo | La app no arranca y dice qué clave falta |
| HTTP en producción | Permitido | Redirige a HTTPS |
| Cookies de sesión y antifalsificación | También viajaban por HTTP | Solo por HTTPS en producción |
| HSTS | No | Sí, en producción |
| `AllowedHosts` | Sin restricción | Solo `academia.liceojavier.edu.gt` en producción |
| Git | `appsettings*.json` excluidos | Se pueden versionar, porque ya no tienen secretos |

## Cómo se hizo

### 1. Secretos de desarrollo en user-secrets

Se ejecutó `dotnet user-secrets init`, que añadió `UserSecretsId` a `SistemaHorarioApp.csproj`. La cadena de conexión y la clave JWT de desarrollo se copiaron sin mostrarlas en pantalla.

Los valores quedan en tu perfil de Windows, fuera del repositorio:

```text
%APPDATA%\Microsoft\UserSecrets\d9cbb9fb-771b-4f0e-8872-8853d6ddcbb3\secrets.json
```

.NET los carga automáticamente cuando el entorno es `Development`. Otro desarrollador debe definir los suyos:

```bash
dotnet user-secrets set "ConnectionStrings:connection" "<cadena de conexión>" --project SistemaHorarioApp
```

```bash
dotnet user-secrets set "Jwt:key" "<clave de 32 bytes o más>" --project SistemaHorarioApp
```

### 2. `appsettings` sin secretos

- `appsettings.json` conserva solo valores no secretos y la configuración de producción.
- `appsettings.Development.json` usa errores detallados, HTTP y `localhost`.
- También se quitaron las cadenas de conexión que estaban comentadas, porque tenían contraseñas.

Los valores anteriores siguen en `E:\DESARROLLO WEB\Time System\CA\APP_backup_antes_clean_arch`. Se comprobó que la conexión y la clave de producción de esa copia son iguales a las que se quitaron.

### 3. Validación al arrancar

`ValidarConfiguracion()` en `SistemaHorario.Infrastructure/DependencyInjection.cs` comprueba:

- que exista `ConnectionStrings:connection`;
- que exista `Jwt:key` con al menos 32 bytes, el mínimo para HMAC-SHA256.

Si algo falla, la app se detiene con un mensaje que nombra la clave, nunca su valor.

### 4. HTTPS configurable y seguro por defecto

`Seguridad:RequiereHttps` en `Program.cs` controla tres cosas: `UseHsts`, `UseHttpsRedirection` y la política `Secure` de las cookies de sesión y antifalsificación.

Si la clave no existe, vale `true`. Solo `appsettings.Development.json` la desactiva, de modo que olvidar la configuración no deja la app insegura.

### 5. `.gitignore`

Ahora excluye `appsettings.Production.json`, `appsettings.Local.json` y `secrets.json`. `appsettings.json` y `appsettings.Development.json` se pueden versionar.

### 6. Script de permisos SQL

[`docs/sql/usuario_aplicacion.sql`](sql/usuario_aplicacion.sql) crea el usuario `app_horarios` con permisos por tabla. **No se ejecutó.** Los permisos se dedujeron del código, así que hay que probarlo antes de retirar el usuario actual.

## Pasos en el servidor

### Prepararlo

1. Instala un certificado HTTPS válido para `academia.liceojavier.edu.gt` y configura el sitio para usarlo.
2. Confirma con qué nombre entran los usuarios. Si es otro, o hay más de uno, define `AllowedHosts` (paso 4). Un nombre no incluido recibe un error 400.
3. Pide al DBA que revise y ejecute el script SQL, con una contraseña nueva.

### 4. Definir las variables de entorno

| Variable | Valor |
|---|---|
| `ConnectionStrings__connection` | Cadena con el usuario nuevo |
| `Jwt__key` | Clave nueva |
| `AllowedHosts` | Solo si cambia el nombre. Varios se separan con `;` |
| `MEDIATR_LICENSE_KEY` | Tu licencia, si tienes una |

Para generar la clave JWT en PowerShell:

```powershell
$b = New-Object byte[] 48; [Security.Cryptography.RandomNumberGenerator]::Create().GetBytes($b); [Convert]::ToBase64String($b)
```

Si el sitio corre en IIS, define las variables en el grupo de aplicaciones. Así no quedan en `web.config`, que se reemplaza al publicar. Sustituye `NOMBRE_DEL_POOL` y ejecuta como administrador:

```powershell
& "$env:windir\system32\inetsrv\appcmd.exe" set config -section:system.applicationHost/applicationPools "/+[name='NOMBRE_DEL_POOL'].environmentVariables.[name='Jwt__key',value='VALOR']" /commit:apphost
```

Repite el comando con `ConnectionStrings__connection`. Los valores que contienen `=` o `;` pueden requerir un escape adicional en `appcmd`; revisa el resultado en `%windir%\system32\inetsrv\config\applicationHost.config`.

### 5. Publicar y comprobar

1. Publica la nueva versión. Reemplazará el `appsettings.json` del servidor, que hoy tiene las contraseñas.
2. Abre `http://` y comprueba que redirige a `https://`.
3. Inicia sesión y prueba crear, aprobar y eliminar un horario, administrar accesos y exportar un reporte.
4. Comprueba que la respuesta HTTPS incluye la cabecera `Strict-Transport-Security`.

### 6. Renovar los secretos

Cambia la contraseña de `siaco` o desactiva ese usuario **cuando el nuevo ya funcione**. Cambia también la contraseña de `sa` en desarrollo y actualiza tu user-secrets. Las contraseñas actuales están copiadas en el respaldo y en las publicaciones anteriores.

Cuando estén renovadas y comprobadas, decide si conservas o eliminas `APP_backup_antes_clean_arch`.

### Validar el certificado de SQL

`TrustServerCertificate=true` desactiva la validación del certificado de SQL Server. Cuando el servidor SQL tenga un certificado válido, cambia ese valor a `false` en la cadena de conexión. Ya no requiere tocar el código.

## Si algo falla

| Síntoma | Causa probable | Qué hacer |
|---|---|---|
| La app no arranca: `Falta la cadena de conexión` o `Falta 'Jwt:key'` | Variable sin definir o no visible para el proceso | Revisa el nombre (lleva dos guiones bajos) y reinicia el grupo de aplicaciones |
| Error 400 en todas las páginas | El nombre del sitio no está en `AllowedHosts` | Agrégalo en la variable `AllowedHosts` |
| El login parece funcionar, pero vuelve a pedir sesión | Se entra por HTTP y la cookie exige HTTPS | Entra por HTTPS y revisa el certificado |
| Error de permisos en SQL | Falta un permiso en el script | Agrega el permiso concreto; no vuelvas a usar `sa` |

**Mientras llega el certificado**, puedes definir `Seguridad__RequiereHttps=false` en el servidor. Las contraseñas de los usuarios viajarían sin cifrar, así que úsalo solo de forma temporal.

## Qué se comprobó

Se ejecutó el programa compilado, con secretos de prueba para el caso de producción:

| Prueba | Resultado |
|---|---|
| Production sin secretos | No arranca y nombra la clave que falta |
| Production con una clave JWT corta | No arranca y explica el motivo |
| Development con user-secrets | Arranca y responde 200 |
| HTTP en Production | 307 a HTTPS |
| HTTPS en Production | 200 |
| Cookie antifalsificación en Production | `secure; samesite=strict; httponly` |
| Host ajeno, en ambos entornos | 400 |
| Búsqueda de contraseñas en los archivos del repositorio | Ninguna |

## Qué no se comprobó

- **Conexión a la base de datos con user-secrets:** la app arrancó, pero ninguna prueba hizo una consulta.
- **Inicio de sesión y cookie de sesión con HTTPS:** requieren un usuario real.
- **Cabecera HSTS:** ASP.NET Core no la envía a `localhost`; hay que comprobarla en el servidor.
- **Publicación e IIS:** los comandos no se ejecutaron y no se confirmó que el servidor use IIS.
- **Script SQL:** no se ejecutó.

## Pendiente de decidir

- **`AllowedHosts`:** el dominio se tomó del `Issuer` de JWT. Confirma que es el nombre real del sitio.
- **JWT:** se sigue generando en cada login aunque nada lo valida. Si se elimina, también desaparece el secreto `Jwt:key`.
