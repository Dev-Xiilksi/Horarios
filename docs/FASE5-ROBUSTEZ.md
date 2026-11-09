# Fase 5: robustez

Qué se hizo, cómo funciona y qué quedó pendiente.

## Resumen

| Tema | Antes | Ahora |
|---|---|---|
| Contexto de base de datos | Uno por conexión de usuario (circuito), compartido por todas las pantallas | Uno por operación, con `IDbContextFactory` |
| Consultas | Muchas usaban `.ToList()` y `.FirstOrDefault()` síncronos dentro de métodos `async` | Todas las de EF Core son asíncronas |
| Validación de datos | Ninguna antes de guardar | FluentValidation en `CrearHorarioCommand`, con mensajes claros |
| Registro de errores | Un `try/catch` repetido en cada handler | Un solo behavior de MediatR para todos los comandos y consultas |
| Bitácora | Si fallaba, la pantalla mostraba error aunque el horario ya estaba guardado | Si falla, se registra en el log y la operación sigue |
| Monitoreo | Ninguno | `/salud` revisa la base de datos y Active Directory |

## 1. Un contexto de base de datos por operación

### El problema

En Blazor Server, los servicios "scoped" viven mientras el usuario tiene la página abierta (todo el circuito). `AcademiaContext` era scoped, así que **todas las pantallas y componentes de un usuario compartían el mismo contexto**. EF Core no permite dos consultas a la vez sobre un mismo contexto. Si el layout y la página consultaban al mismo tiempo, aparecía el error *"A second operation was started on this context instance before a previous operation completed"*. Además, el contexto acumulaba entidades durante toda la sesión.

### Cómo se resolvió

- **Registro:** `AddInfrastructure()` usa `AddDbContextFactory<AcademiaContext>` en lugar de `AddDbContext`.
- **Clases afectadas:** los 23 repositorios y servicios de consulta de Infrastructure reciben `IDbContextFactory<AcademiaContext>`.
- **Uso:** cada método crea su contexto y lo libera al terminar:

```csharp
public async Task<List<AreaDTO>> GetAll()
{
    await using var _context = await _contextFactory.CreateDbContextAsync();
    ...
}
```

- **Herramienta:** el cambio se aplicó con un script y después se ajustaron a mano los casos especiales.

### Casos especiales

- **Aprobación de horarios:** antes se cargaba el horario en una llamada y se guardaba en otra con el mismo contexto. Ahora `Buscar` lo lee sin seguimiento (`AsNoTracking`). `GuardarAprobacion(horario, confirmacion)` lo adjunta a un contexto nuevo y **solo marca `IdConfirmacion` como modificado**. Las demás columnas no se tocan, y el cambio de estado y la confirmación se guardan en un solo `SaveChanges`, que es una transacción.
- **Accesos de usuario:** `Actualizar` y `Eliminar` reciben la entidad y la adjuntan al contexto nuevo.
- **Consulta de accesos con nombres:** `ConsultaConNombres` devolvía un `IQueryable` del contexto compartido. Ahora recibe el contexto como parámetro.
- **Métodos sin `async`:** cinco métodos devolvían la tarea de la consulta sin esperarla. Con un contexto por método, el contexto se habría liberado antes de que terminara la consulta. Ahora son `async` y esperan el resultado.

### Consultas asíncronas

`.ToList()` y `.FirstOrDefault()` al final de las consultas de EF Core pasaron a `ToListAsync()` y `FirstOrDefaultAsync()`. El hilo del servidor ya no queda bloqueado mientras responde SQL Server.

## 2. Behaviors de MediatR

Todos los comandos y consultas de `UseCases` pasan por dos behaviors, registrados en `AddApplication()` en este orden:

```text
ErroresBehavior ──> ValidacionBehavior ──> handler
```

| Behavior | Qué hace |
|---|---|
| `Comportamientos/ValidacionBehavior.cs` | Ejecuta los validadores de FluentValidation de la petición. Si hay errores, lanza `DatosInvalidosException` con todos los mensajes y el handler no se ejecuta |
| `Comportamientos/ErroresBehavior.cs` | Si algo falla, registra el error en el log y en `hr_log_errors` con el usuario de la sesión, y lo vuelve a lanzar. La falta de permiso y los datos inválidos son respuestas esperadas: no se registran como error |

Así se quitaron los `try/catch` que registraban el error en cada handler. `EliminarHorarioCommand` conserva el suyo porque convierte el error en una respuesta con mensaje (`ResponseDTO`), que es lo que espera la pantalla.

Si guardar el error en `hr_log_errors` falla, eso no oculta el error original.

### Validación de crear horario

`CrearHorarioValidator` está en `UseCases/Horario/Commands/CrearHorario.cs`, junto al comando. Comprueba lo siguiente:

- Que el horario venga completo, con empleado y ciclo.
- Que el tipo de horario sea fijo (1) o variable (2), los dos valores que usan las pantallas.
- Que ningún grado esté vacío.
- Que cada franja tenga horas en formato `hh:mm:ss` y que la hora final no sea anterior a la inicial. El mensaje indica la franja, por ejemplo: "Franja 3: la hora final (08:00:00) es anterior a la inicial (09:00:00)."

Antes, una hora invertida producía un error técnico del value object `Horario`. Ahora el usuario ve qué franja corregir.

Para validar otro comando, crea una clase `AbstractValidator<TuComando>` en Application. Se registra sola.

## 3. Bitácora que no rompe la operación

### Por qué cambió el plan

El plan proponía guardar el horario y su bitácora en una sola transacción. No fue posible de forma segura:

- La bitácora (`hr_bitacora`) se escribe con Dapper y su propia conexión, y no es una entidad de EF Core.
- Unirlas exigiría una transacción distribuida o mapear la tabla en EF Core, y no se conoce su estructura exacta (clave, columnas).

### Lo que se hizo

- **Lo que ya estaba en una transacción:** el horario, sus grados y sus franjas se guardan en un solo `SaveChanges`. La aprobación y su confirmación también.
- **Bitácora:** se escribe después. Si falla, `HrBitacoraService` lo registra en el log con la acción, el horario y el usuario, y **la operación no se marca como fallida**. Antes, el usuario veía un error de algo que sí se había guardado y podía intentarlo de nuevo.
- **Riesgo que queda:** puede faltar un registro de bitácora si la base de datos falla justo en ese momento. Queda constancia en el log.

## 4. Salud del sistema

`GET /salud` responde `Healthy`, `Degraded` o `Unhealthy`, sin detalles internos.

| Check | Qué comprueba |
|---|---|
| `base-de-datos` | Que EF Core pueda conectarse a la base |
| `directorio-activo` | Que se pueda contactar un controlador del dominio (`DirectorioActivoHealthCheck`, en `Infrastructure/Salud`) |

Sirve para que un monitor del servidor o el balanceador avisen si algo cae. No requiere sesión y responde solo para los hosts permitidos (`AllowedHosts`).

## Tests

Hay 5 tests nuevos en `ComportamientosTests`, con 36 en total. Pasan por MediatR con el mismo registro que usa la app:

- Una hora final anterior a la inicial se rechaza con el mensaje de la franja y no se guarda nada.
- Un tipo de horario o un ciclo inválidos se rechazan.
- Un horario válido pasa la validación y se guarda.
- Un error inesperado se registra en `hr_log_errors` con la petición y el usuario, y se vuelve a lanzar.
- La falta de permiso no se registra como error.

Estos tests encontraron un caso frágil: si al releer el horario recién creado no se obtenía resultado, el handler fallaba con `NullReferenceException` aunque el horario ya estaba guardado. Ahora responde con los datos guardados.

## Qué se comprobó

| Prueba | Resultado |
|---|---|
| Compilación | Sin errores (141 advertencias, antes 157) |
| Tests | 36 de 36 |
| App en Development | Arranca y el login responde 200 |
| `/salud` contra la base de desarrollo | `Healthy`: la base de datos, ya con la fábrica de contextos, y Active Directory responden |
| 20 consultas simultáneas sobre la misma instancia, contra la base de desarrollo, solo lectura | Todas correctas (16 horarios de 2026 y 19 áreas) |

## Qué no se comprobó

- **El error anterior:** no se reprodujo con la versión vieja. Ese error de EF Core está documentado.
- **Pantallas con usuarios reales:** crear, aprobar y eliminar horarios, y la gestión de accesos.
- **`/salud` en el servidor de producción.**

## Pendiente

- **Advertencias:** quedan 141, casi todas de nulabilidad en las páginas Razor y en los DTO. No afectan al funcionamiento, pero conviene revisarlas.
- **`DataToChartsService`:** usa Dapper y ya era asíncrono. Sigue sin comprobar permisos (ver Fase 3).
- **Validadores de otros comandos:** solo crear horario tiene validador.
- **Bitácora:** si se confirma la estructura de `hr_bitacora`, se puede mapear en EF Core y guardarla en la misma transacción que el horario.
