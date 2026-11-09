# Cambios

Resumen de la refactorización, de las Fases 1, 2 y 3 de seguridad, del límite de intentos de la Fase 4 y de la Fase 5 de robustez, de octubre de 2026. El estado anterior está en `E:\DESARROLLO WEB\Time System\CA\APP_backup_antes_clean_arch`.

## Correspondencia con los proyectos anteriores

| Antes | Ahora |
|---|---|
| `Models` | `SistemaHorario.Domain` |
| `DTO` | `SistemaHorario.Application/DTOs` |
| `BLL/Service/Interfaces` | `SistemaHorario.Application/Interfaces` |
| `BLL/Service` | `Application/Services`, `Application/UseCases/Horario` o `Infrastructure/Queries`, según el caso |
| `DAL` | `SistemaHorario.Infrastructure/Persistence` |
| `Utility/ConfigureJWT` | `Infrastructure/Security/JwtTokenGenerator` |
| `IOC/Dependency` | `DependencyInjection.cs` en Application e Infrastructure |
| `SistemaHorarioApp/Servicios` | `Infrastructure/Reports` |

## Añadido

**CQRS de horarios**
- MediatR 14.2.0, con 3 comandos y 5 consultas en `Application/UseCases/Horario`.
- `IHorarioRepository` para escribir e `IHorarioConsultas` para leer.

**Reglas y contratos**
- Constantes: `EstadoConfirmacion`, `TipoFranja`, `RolesSistema`.
- Value objects: `CicloEscolar`, `NivelGrado`.
- Reglas en entidades: `HrHorario1.Reglas.cs`, `AccesoUsuario.Reglas.cs`.
- Repositorios: `ReporteRepository`, `UsuarioRepository`, `AccesoUsuarioRepository`, `ActividadRepository`.
- Contratos para lo externo: `IDirectorioActivoService`, `IJwtTokenGenerator`, `IEmailService`, `IHrGradosService`.
- `HorarioSemanalBuilder`, que arma la grilla semanal.
- `ExcelHorarioService.GenerarExcelResumen`.

**Seguridad (Fase 1)**
- `LoginTicketStore` y canje del ticket en `/Account/Login`.
- `AuthorizeRouteView`, exigencia de sesión por defecto, `RedirectToLogin` y `SinPermiso`.
- Opciones de cookie y cierre de sesión por POST.

**Proyecto**
- Repositorio git con `.gitignore`.
- Esta documentación.

**Secretos y configuración (Fase 2)**
- `dotnet user-secrets` en desarrollo y variables de entorno en producción.
- Validación de secretos al arrancar.
- HTTPS, HSTS, cookies solo por HTTPS y hosts permitidos en producción.
- Script `docs/sql/usuario_aplicacion.sql`, sin ejecutar.
- Detalle en [FASE2-SECRETOS-Y-CONFIGURACION.md](FASE2-SECRETOS-Y-CONFIGURACION.md).

**Reglas por rol (Fase 3)**
- `IUsuarioActual`, `ReglasDeAcceso` y `AccesoDenegadoException`.
- Permisos en los comandos y consultas de horarios, en reportes y en la administración de accesos.
- Proyecto `SistemaHorario.Tests` con 22 tests.
- Detalle en [FASE3-REGLAS-POR-ROL.md](FASE3-REGLAS-POR-ROL.md).

**Límite de intentos de inicio de sesión (Fase 4)**
- `LimitadorIntentosLogin`: 5 intentos fallidos por usuario y 20 por IP cada 15 minutos, comprobados antes de consultar Active Directory.
- `AddRateLimiter` en `/Account/Login` y `/Account/Logout`: 10 peticiones por minuto por IP.
- 9 tests nuevos (31 en total).
- Detalle en [FASE4-LIMITE-DE-INTENTOS.md](FASE4-LIMITE-DE-INTENTOS.md).

**Robustez (Fase 5)**
- Un contexto de base de datos por operación (`IDbContextFactory`) y consultas asíncronas.
- Behaviors de MediatR: `ValidacionBehavior` (FluentValidation) y `ErroresBehavior`.
- La bitácora ya no hace fallar la operación principal.
- `/salud` revisa la base de datos y Active Directory.
- 5 tests nuevos (36 en total).
- Detalle en [FASE5-ROBUSTEZ.md](FASE5-ROBUSTEZ.md).

## Eliminado

| Qué | Motivo |
|---|---|
| Proyectos `BLL`, `DAL`, `DTO`, `IOC`, `Models`, `Utility` | Reemplazados por las nuevas capas |
| `IHrHorario1Service`, `IHrHorario2Service` | Reemplazados por comandos y consultas |
| `IHrHorarioExtraService` | Se inyectaba, pero nunca se llamaba |
| `BitacoraConfirmacionService` | Sin uso y siempre devolvía `null` |
| `IHrHorarioExtra`, `IEmpleadoService`, `IEmpresaService` | Interfaces sin uso |
| 25 métodos que lanzaban `NotImplementedException` | Sin implementar y sin llamadas desde la UI |
| `/test`, `/testss`, `/counter`, `ConsultarTest` | Páginas de prueba; `/test` podía crear y borrar horarios |

## Cambios de comportamiento

| Qué | Antes | Ahora |
|---|---|---|
| Iniciar sesión | La URL definía usuario y rol | Requiere un ticket emitido tras validar las credenciales |
| Navegar sin sesión | Algunas páginas abrían | Todas redirigen al login, salvo las públicas |
| `/acceso-usuario` | Cualquier usuario con sesión | Solo Administrador |
| Cerrar sesión | GET | POST con token |
| Maestro sin acceso registrado | Error en pantalla | "No tienes permiso para acceder." |
| Fallo de Active Directory al iniciar sesión | Error sin controlar | Mensaje genérico |
| Crear horario con hora final anterior a la inicial | Guardaba un tiempo incorrecto | Se rechaza con error |
| Exportar el Excel de un empleado sin horario | `NullReferenceException` | Error que explica la causa |
| Respuesta de crear horario | Sin `TipoFranja` | Incluye `TipoFranja` |
| Arrancar sin secretos | Arrancaba y fallaba después | No arranca y nombra la clave que falta |
| Entrar por HTTP en producción | Permitido | Redirige a HTTPS |
| Entrar con otro nombre de host en producción | Permitido | Error 400 |
| Aprobar horario | Usaba el coordinador y los datos que enviaba la pantalla; dejaba algunos campos en `null` | Usa el usuario de la sesión y solo cambia el estado |
| Ver o modificar horarios ajenos sin permiso | Posible fuera de la pantalla prevista | Error de permiso |
| Muchos intentos fallidos de inicio de sesión | Sin límite; cada intento llegaba al AD | Tras 5 fallos por usuario o 20 por IP: "Demasiados intentos", sin consultar el AD |
| Hora final anterior a la inicial al crear un horario | Error técnico | Mensaje con la franja a corregir |
| Falla la bitácora después de guardar | La pantalla mostraba error aunque el horario estaba guardado | La operación termina bien y el fallo queda en el log |
| Consultas simultáneas del mismo usuario | Podían fallar por compartir el contexto de base de datos | Cada operación usa su propio contexto |

El resto de las consultas y de la lógica se trasladó sin cambios a propósito.

## Verificación

- La solución compila sin errores.
- La app arranca en Development y responde.
- Los casos de uso nuevos se probaron con repositorios falsos en un programa temporal. **El proyecto todavía no tiene tests automatizados.**
- **Nada de esto se ha probado contra la base de datos ni con un inicio de sesión real.** Los pasos pendientes están en [SEGURIDAD.md](SEGURIDAD.md).

## Pendiente de confirmar

- Los nombres `Pendiente` y `NoConfirmado` de `EstadoConfirmacion`.
- El enlace `/page/data` del menú no tiene una página que lo atienda.
- Si un maestro sin acceso registrado debería poder entrar. Hoy no puede.
- Las Fases 1 a 5 aún no tienen commit.
