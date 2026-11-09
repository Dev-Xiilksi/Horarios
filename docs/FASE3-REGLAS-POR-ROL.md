# Fase 3: reglas por rol en el servidor

Qué se hizo, cómo funciona y qué falta.

## El problema

Hasta la Fase 2, los permisos de las acciones dependían de la pantalla:

- `AprobarHorarioCommand` tomaba `IdCoordinador` y todos los campos del horario desde la pantalla, y los guardaba tal cual.
- `EliminarHorarioCommand` y `CrearHorarioCommand` aceptaban cualquier número de empleado.
- Las consultas devolvían todos los horarios, y cada pantalla los filtraba por área.

Cualquier forma de llamar a esos comandos fuera de la pantalla prevista se saltaba esas restricciones.

## Cómo se decidieron las reglas

Las reglas reproducen lo que las pantallas ya permitían. No se inventó ninguna regla nueva; ahora las aplica el servidor.

- **InicioCoordinador:** un coordinador ve solo los horarios de su área, salvo si su área es la 0 o la 14, que ven todo.
- **InicioAdminConsultor y DatosPorArea:** el Administrador y el Consultor ven todo y pueden aprobar.
- **Pantallas de registro:** el educador crea y elimina su propio horario.

## Reglas

Están en `SistemaHorario.Application/Seguridad/ReglasDeAcceso.cs`.

| Acción | Administrador | Consultor | Coordinador del área 0 o 14 | Otro coordinador | Educador |
|---|---|---|---|---|---|
| Ver un horario | Todos | Todos | Todos | Su área y el propio | Solo el propio |
| Lista de horarios del ciclo | Todos | Todos | Todos | Su área y el propio | Solo el propio |
| Aprobar | Sí | Sí | Sí | Solo su área | No |
| Crear o eliminar horario | Cualquiera | Solo el propio | Solo el propio | Solo el propio | Solo el propio |
| Consolidado de un área | Sí | Sí | Sí | Solo su área | No |
| Reportes de todas las áreas | Sí | Sí | Sí | No | No |
| Crear, editar o eliminar accesos | Sí | No | No | No | No |
| Leer accesos | Sí | Sí | Sí | Sí | Sí |

Cualquier usuario puede ver su propio horario.

Leer accesos sigue abierto porque las pantallas de inicio lo usan para saber el área del usuario.

## Cómo funciona

### Quién es el usuario

```text
Cookie de sesión ──> UsuarioActualBlazor (UI) ──> IUsuarioActual (Application) ──> handlers y servicios
```

- `IUsuarioActual` (Application) entrega `IdEmpleado`, `Rol` e `IdArea` de la sesión.
- `UsuarioActualBlazor` (`SistemaHorarioApp/Seguridad`) lo lee con `AuthenticationStateProvider`, porque en Blazor Server no hay `HttpContext` durante el circuito.
- El área viene del dato `idArea` que guarda el login desde la Fase 1.

### Dónde se aplica cada regla

| Archivo | Comprobación |
|---|---|
| `AprobarHorario.cs` | Carga el horario de la base de datos, revisa su área y aprueba en nombre del usuario de la sesión |
| `CrearHorario.cs` | Solo el propio horario, salvo el Administrador |
| `EliminarHorario.cs` | Igual. Si no hay permiso, responde con `status = false` y un mensaje, como en los otros casos de este comando |
| `ObtenerHorarios.cs` | Filtra la lista según el rol |
| `ObtenerHorarioPorEmpleado.cs`, `ObtenerHorarioPorId.cs`, `ObtenerFranjasDelHorario.cs` | Revisan a quién pertenece el horario |
| `ObtenerHorariosNoConfirmados.cs` | Revisa el área pedida |
| `ReporteService.cs` | Excel por empleado, consolidado por área y reportes de todas las áreas |
| `AccesoUsuarioService.cs` | Crear, editar y eliminar, solo el Administrador |

Si no hay permiso, se lanza `AccesoDenegadoException` con un mensaje que se puede mostrar. Las pantallas ya tenían `try/catch` en esas llamadas, así que el mensaje aparece en su alerta de error.

### Cambios en la aprobación

- `AprobarHorarioCommand` ahora recibe **solo el id del horario**: `new AprobarHorarioCommand(obj.IdHorario1)`.
- El repositorio obtiene el horario con `Buscar` y solo cambia `IdConfirmacion`.
- Antes se guardaba el horario completo que enviaba la pantalla. Como ese objeto no traía sección, corrección de trabajos, estudiantes ni observación, **aprobar dejaba esos campos en `null`**. Eso ya no ocurre.
- Aprobar un horario ya aprobado no crea una segunda confirmación.

### Tests

Se creó el proyecto `SistemaHorario.Tests` (xUnit), con 22 tests:

- `ReglasDeAccesoTests`: la tabla de reglas con los cuatro roles.
- `HorarioHandlersTests`: aprobar, crear, eliminar, listar y ver horarios, y modificar accesos, con repositorios falsos.

```bash
dotnet test SistemaHorario.Tests
```

## Cambios de comportamiento

| Qué | Antes | Ahora |
|---|---|---|
| Quién aprueba | El `IdCoordinador` que mandaba la pantalla | El usuario de la sesión |
| Datos del horario al aprobar | Se sobrescribían con los de la pantalla; algunos quedaban en `null` | Solo cambia el estado de confirmación |
| Aprobar dos veces | Creaba dos confirmaciones | Crea una |
| Bitácora de crear y eliminar | Registraba al empleado del horario | Registra a quien hizo la acción |
| Administrador | Sin regla explícita | Puede crear y eliminar el horario de cualquier empleado |
| Coordinador de un área normal | Recibía todos los horarios y la pantalla los filtraba | Recibe solo su área y los propios |
| Educador que abre `/page/horario?e=otro` | Veía el horario de otro | Recibe un error de permiso |

Para un usuario normal, las pantallas se comportan igual que antes.

## Qué se comprobó

- La solución compila sin errores.
- Los 22 tests pasan.
- La app arranca en Development, el login responde 200 y `/page/home` sin sesión redirige al login.

## Qué no se comprobó

- **Pantallas con usuarios reales de cada rol:** aprobar como coordinador de su área y de otra, eliminar como educador y abrir `/page/horario?e=` con el número de otro empleado.
- **Datos reales:** las reglas usan el área del horario (`hr_horario1.area`) y la del acceso del coordinador (`AccesoUsuario.IdArea`), igual que las pantallas. No se comprobó que coincidan en los datos reales.

## Pendiente

### Lo que no cubre esta fase

Estos servicios de solo lectura de `Infrastructure/Queries` todavía no comprueban quién los pide:

| Servicio | Lo usa | Riesgo |
|---|---|---|
| `DataToChartsService.OnlyHours(empleado)` y `GetHour(empleado)` | `/dash` y gráficas | Ver las horas de cualquier empleado |
| `DataToChartsService.OnlyHoursByArea(area)` | Pantallas de inicio y de área | Ver el total de horas de cualquier área |
| `ListadoHorarioColaboradorService` | Inicio del educador y registro | Datos de cualquier colaborador |
| `EmployeeProfileService`, `EmplegenService` | Varias pantallas | Datos de empleados |

Para protegerlos, el siguiente paso es moverlos a consultas de Application con `IUsuarioActual`, como se hizo con los horarios.

### Decisiones pendientes

- **¿Debe el Consultor aprobar horarios?** Hoy puede, porque así lo permitía la pantalla. Si no debe, el cambio es una línea en `ReglasDeAcceso.PuedeAprobar`.
- **Áreas con visión global:** la 0 y la 14 están en `Domain/Constants/AreasEspeciales.cs`. Un comentario del código antiguo mencionaba también la 11 y la 16, pero las pantallas solo usaban la 0 y la 14.
- **Comportamiento común en MediatR:** el plan lo incluía para exigir un rol a ciertos comandos. No se hizo porque todas las reglas actuales dependen del horario concreto (su área o su dueño), y eso solo se puede comprobar dentro del handler.
