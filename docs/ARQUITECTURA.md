# Arquitectura

La solución sigue Clean Architecture: las dependencias apuntan hacia el dominio.

```text
SistemaHorarioApp (UI)  ──> SistemaHorario.Application ──> SistemaHorario.Domain
        │                              ▲                          ▲
        └──> SistemaHorario.Infrastructure ───────────────────────┘
```

La UI referencia Infrastructure solo para registrar servicios con `AddInfrastructure()` en `Program.cs`. Application y Domain no conocen EF Core, Dapper ni Active Directory.

## Proyectos

| Proyecto | Contiene | Carpetas |
|---|---|---|
| `SistemaHorario.Domain` | Entidades y reglas sin dependencias externas | `Entities/`, `ValueObjects/`, `Constants/` |
| `SistemaHorario.Application` | Casos de uso, permisos, validación, contratos y DTOs | `UseCases/`, `Services/`, `Seguridad/`, `Comportamientos/`, `Interfaces/`, `DTOs/` |
| `SistemaHorario.Infrastructure` | Base de datos y servicios externos | `Persistence/`, `Repositories/`, `Queries/`, `Auditoria/`, `Security/`, `Email/`, `Reports/`, `Salud/` |
| `SistemaHorarioApp` | Pantallas Blazor, login y arranque | `Components/`, `Dialog/`, `Pages/Account/`, `Seguridad/` |
| `SistemaHorario.Tests` | Tests de Application | `Fakes/` |

El proyecto `Client` no forma parte de la solución ni se usa.

## Tres formas de implementar una funcionalidad

| Caso | Patrón | Ejemplos |
|---|---|---|
| Horarios | CQRS con MediatR | `UseCases/Horario/` |
| Otra funcionalidad con reglas de negocio | Servicio en Application + repositorio en Infrastructure | Login, accesos, actividades, reportes |
| Solo lectura, sin reglas | Servicio en `Infrastructure/Queries` que implementa una interfaz de Application | Áreas, grados, niveles, gráficas |

Si un servicio de solo lectura empieza a tener reglas, pasa a la segunda fila.

## CQRS de horarios

Cada archivo de `UseCases/Horario` contiene el `record` de la petición y su handler.

| Tipo | Petición | Qué hace |
|---|---|---|
| Comando | `CrearHorarioCommand` | Registra el horario como pendiente y escribe la bitácora |
| Comando | `AprobarHorarioCommand` | Aprueba el horario y guarda la confirmación |
| Comando | `EliminarHorarioCommand` | Elimina el horario del ciclo actual, salvo que esté aprobado |
| Consulta | `ObtenerHorariosQuery` | Horarios de un ciclo |
| Consulta | `ObtenerHorariosNoConfirmadosQuery` | Horarios no confirmados por área y ciclo |
| Consulta | `ObtenerHorarioPorIdQuery` | Un horario por id |
| Consulta | `ObtenerHorarioPorEmpleadoQuery` | Horario de un empleado en un ciclo |
| Consulta | `ObtenerFranjasDelHorarioQuery` | Franjas de un horario |

La escritura y la lectura usan contratos separados:

- `IHorarioRepository`, implementado en `Repositories/HorarioRepository`, trabaja con entidades y lo usan los comandos.
- `IHorarioConsultas`, implementado en `Queries/HorarioConsultas`, devuelve DTOs y lo usan las consultas.

Desde una pantalla:

```razor
@inject ISender _sender

var horarios = await _sender.Send(new ObtenerHorariosQuery(ciclo));
```

Los handlers se registran solos en `AddApplication()`. `_Imports.razor` ya incluye los namespaces de comandos y consultas.

El namespace `UseCases.Horario` choca con el value object `Horario`. Dentro de esos handlers hay que usar un alias, como en `CrearHorario.cs`:

```csharp
using HorarioVO = SistemaHorario.Domain.ValueObjects.Horario;
```

## Reglas en el dominio

| Elemento | Regla |
|---|---|
| `EstadoConfirmacion` | Da nombre a los valores 0, 1 y 2 de `IdConfirmacion` |
| `TipoFranja` | 1 es horario fijo; cualquier otro valor es variable |
| `RolesSistema` | Nombres de los roles |
| `CicloEscolar.Actual()` | El ciclo es el año en curso |
| `NivelGrado` | Separa un código: `P10` es nivel `P`, grado `10` |
| `Horario` | La hora final no puede ser anterior a la inicial; calcula el tiempo de cada franja |
| `HrHorario1.Aprobar()` | Marca el horario como aprobado y crea el registro de confirmación |
| `AccesoUsuario.Actualizar()` | Cambia rol y área, y registra la fecha |

Los nombres `Pendiente` (0) y `NoConfirmado` (2) se dedujeron del código. Falta confirmarlos con quien conozca los datos.

## Base de datos

Infrastructure recibe `IDbContextFactory<AcademiaContext>` y crea un contexto por método (`await using var _context = await _contextFactory.CreateDbContextAsync();`). No inyectes `AcademiaContext` directamente: en Blazor Server viviría toda la sesión del usuario. Detalle en [FASE5-ROBUSTEZ.md](FASE5-ROBUSTEZ.md).

## Behaviors de MediatR

Todo comando o consulta pasa por `ErroresBehavior` (registra errores inesperados) y `ValidacionBehavior` (ejecuta los validadores de FluentValidation). No repitas `try/catch` para registrar errores en los handlers. Para validar un comando, crea un `AbstractValidator<TuComando>` en Application; se registra solo.

## Permisos

Las reglas de quién puede hacer qué están en `Application/Seguridad/ReglasDeAcceso.cs`. Los handlers y servicios obtienen el usuario con `IUsuarioActual` y lanzan `AccesoDenegadoException` si no tiene permiso. Nunca tomes el usuario, el rol o el área de lo que manda la pantalla. Detalle en [FASE3-REGLAS-POR-ROL.md](FASE3-REGLAS-POR-ROL.md).

## Tests

`SistemaHorario.Tests` (xUnit) prueba Application con repositorios falsos, en `Fakes/`. Ejecuta `dotnet test SistemaHorario.Tests`.

## Dónde poner algo nuevo

1. **Entidad o regla sin dependencias:** `Domain`.
2. **Operación de horarios:** un archivo nuevo en `UseCases/Horario/Commands` o `Queries`.
3. **Consulta o guardado en base de datos:** el contrato en `Application/Interfaces` y la implementación en `Infrastructure`, registrada en su `DependencyInjection.cs`.
4. **Pantalla:** `SistemaHorarioApp/Components/Pages`. Exige sesión por defecto. Si necesita un rol, usa `@attribute [Authorize(Roles = RolesSistema.Administrador)]`.
