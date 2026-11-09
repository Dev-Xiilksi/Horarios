/*
  Usuario de base de datos propio de la aplicacion, con permisos minimos.
  Lo debe ejecutar un DBA. Revisar primero en un ambiente de prueba.

  Antes de ejecutar:
    1. Reemplazar <CONTRASENA_FUERTE> por una contrasena generada. No guardar el valor en este archivo.
    2. Confirmar que las tablas estan en el esquema dbo.

  Los permisos salen de las consultas actuales del codigo. Probar crear, aprobar y eliminar un horario,
  administrar accesos y exportar reportes antes de retirar el usuario anterior.
*/

USE [master];
CREATE LOGIN [app_horarios] WITH PASSWORD = N'<CONTRASENA_FUERTE>', CHECK_POLICY = ON;
GO

USE [academia];
CREATE USER [app_horarios] FOR LOGIN [app_horarios];
GO

-- Solo lectura: catalogos y datos de empleados
GRANT SELECT ON dbo.areas TO [app_horarios];
GRANT SELECT ON dbo.COLEGIOS TO [app_horarios];
GRANT SELECT ON dbo.CatalogoColegio TO [app_horarios];
GRANT SELECT ON dbo.contratos1 TO [app_horarios];
GRANT SELECT ON dbo.dias TO [app_horarios];
GRANT SELECT ON dbo.emplegen TO [app_horarios];
GRANT SELECT ON dbo.empresas TO [app_horarios];
GRANT SELECT ON dbo.GRADOS TO [app_horarios];
GRANT SELECT ON dbo.MAESTROS TO [app_horarios];
GRANT SELECT ON dbo.MAESTROSPORSECCION TO [app_horarios];
GRANT SELECT ON dbo.MATERIAS TO [app_horarios];
GRANT SELECT ON dbo.MATERIASPORGRADO TO [app_horarios];
GRANT SELECT ON dbo.NIVELES TO [app_horarios];
GRANT SELECT ON dbo.rol TO [app_horarios];
GRANT SELECT ON dbo.hr_actividades TO [app_horarios];
GRANT SELECT ON dbo.hr_eventos TO [app_horarios];
GRANT SELECT ON dbo.hr_frecuencia TO [app_horarios];
GRANT SELECT ON dbo.hr_general TO [app_horarios];
GRANT SELECT ON dbo.hr_general2 TO [app_horarios];
GRANT SELECT ON dbo.hr_tipo_franja TO [app_horarios];
GRANT SELECT ON dbo.hr_variable TO [app_horarios];
GO

-- Lectura y escritura: horarios y accesos
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.hr_horario1 TO [app_horarios];
GRANT SELECT, INSERT, DELETE ON dbo.hr_horario2 TO [app_horarios];
GRANT SELECT, INSERT, DELETE ON dbo.hr_grados TO [app_horarios];
GRANT SELECT, DELETE ON dbo.hr_horario_extra TO [app_horarios];
GRANT SELECT, INSERT, DELETE ON dbo.hr_bitacora_confirmacion TO [app_horarios];
GRANT SELECT, INSERT, UPDATE, DELETE ON dbo.AccesoUsuario TO [app_horarios];
GO

-- Solo escritura: bitacora y registros
GRANT INSERT ON dbo.hr_bitacora TO [app_horarios];
GRANT INSERT ON dbo.hr_log_errors TO [app_horarios];
GRANT INSERT ON dbo.hr_correos TO [app_horarios];
GRANT INSERT ON dbo.app_log TO [app_horarios];
GO
