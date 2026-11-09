# Sistema de Horarios

Aplicación web para registrar, aprobar y consultar los horarios de los educadores. Blazor Server sobre .NET 10, SQL Server y Active Directory.

## Documentación

- [Arquitectura](docs/ARQUITECTURA.md): capas, CQRS y dónde poner cada cosa.
- [Seguridad](docs/SEGURIDAD.md): login, permisos por página y lo que falta.
- [Cambios](docs/CAMBIOS.md): qué se añadió, qué se eliminó y qué cambió de comportamiento.
- [Fase 2: secretos y configuración](docs/FASE2-SECRETOS-Y-CONFIGURACION.md): qué se hizo, cómo y los pasos en el servidor.
- [Fase 3: reglas por rol](docs/FASE3-REGLAS-POR-ROL.md): quién puede hacer qué y cómo lo comprueba el servidor.
- [Fase 4: límite de intentos](docs/FASE4-LIMITE-DE-INTENTOS.md): cómo se limitan los intentos de inicio de sesión.
- [Fase 5: robustez](docs/FASE5-ROBUSTEZ.md): contexto de base de datos por operación, validación, errores, bitácora y `/salud`.

## Poner en marcha

Requisitos: SDK de .NET 10, acceso a SQL Server y equipo o servidor en el dominio.

Los secretos no están en el repositorio. En desarrollo van en `dotnet user-secrets` y en producción en variables de entorno. La app no arranca si falta alguno de los obligatorios:

| Clave | Uso | Obligatoria |
|---|---|---|
| `ConnectionStrings:connection` | Base de datos | Sí |
| `Jwt:key`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationMinutes` | Token que genera el login. `Jwt:key` es secreta y debe tener al menos 32 bytes; el resto está en `appsettings` | Sí |
| `ActiveDirectory:Domain` | Dominio para validar usuarios. Si falta, se usa el dominio del equipo | No |
| `Seguridad:RequiereHttps` | Exige HTTPS. Vale `true` si falta; `appsettings.Development.json` lo desactiva | No |
| `MediatR:LicenseKey` | Licencia de MediatR. También se puede dar con la variable de entorno `MEDIATR_LICENSE_KEY` | No |
| `EmailSettings` | `Email`, `Password`, `Host`, `Port`. Solo para `EmailService`, que hoy ninguna pantalla usa | No |

```bash
dotnet build sistema_horarios.sln
```

```bash
dotnet test SistemaHorario.Tests
```

```bash
dotnet run --project SistemaHorarioApp
```
