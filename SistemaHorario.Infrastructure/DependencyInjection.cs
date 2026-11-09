using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Auditoria;
using SistemaHorario.Infrastructure.Email;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Infrastructure.Queries;
using SistemaHorario.Infrastructure.Reports;
using SistemaHorario.Infrastructure.Repositories;
using SistemaHorario.Infrastructure.Salud;
using SistemaHorario.Infrastructure.Security;

namespace SistemaHorario.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            ValidarConfiguracion(configuration);

            //persistencia
            //un contexto por operacion: en Blazor Server un contexto compartido por circuito falla con consultas simultaneas
            services.AddDbContextFactory<AcademiaContext>(option =>
            {
                option.UseSqlServer(configuration.GetConnectionString("connection"));
            });

            services.AddScoped<DapperContext>();

            #region "AddScoped"
            //repositorios (usados por los servicios de Application)
            services.AddScoped<IHorarioRepository, HorarioRepository>();
            services.AddScoped<IReporteRepository, ReporteRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAccesoUsuarioRepository, AccesoUsuarioRepository>();
            services.AddScoped<IActividadRepository, ActividadRepository>();

            //consultas de solo lectura
            services.AddScoped<IHorarioConsultas, HorarioConsultas>();
            services.AddScoped<IHrFrecuenciaService, HrFrecuenciaService>();
            services.AddScoped<IHrGeneralService, HrGeneralService>();
            services.AddScoped<IHrGeneral2Service, HrGeneral2Service>();
            services.AddScoped<IColegioService, ColegioService>();
            services.AddScoped<INivelesService, NivelesService>();
            services.AddScoped<IGradosService, GradosService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IEmplegenService, EmplegenService>();
            services.AddScoped<IMaestrosPorSeccionService, MaestrosPorSeccionService>();
            services.AddScoped<IDiasService, DiasService>();
            services.AddScoped<IColegioNivelGradoSeccionService, ColegioNivelGradoSeccionService>();
            services.AddScoped<IListadoHorarioColaboradorService, ListadoHorarioColaboradorService>();
            services.AddScoped<IHrVariableService, HrVariableService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IEmployeeProfileService, EmployeeProfileService>();
            services.AddScoped<IDataToChartsService, DataToChartsService>();
            services.AddScoped<IHrGradosService, HrGradosService>();

            //auditoria
            services.AddScoped<ILogErrorService, LogErrorService>();
            services.AddScoped<IHrBitacoraService, HrBitacoraService>();

            //servicios externos
            services.AddScoped<IDirectorioActivoService, DirectorioActivoService>();
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IExcelHorarioService, ExcelHorarioService>();

            //salud: la UI publica el resultado en /salud
            services.AddHealthChecks()
                .AddDbContextCheck<AcademiaContext>("base-de-datos")
                .AddCheck<DirectorioActivoHealthCheck>("directorio-activo");

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.MSSqlServer(
                    connectionString: configuration.GetConnectionString("connection"),
                    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
                    {
                        TableName = "app_log",
                        AutoCreateSqlTable = false
                    })
                    .CreateLogger();
            #endregion

            return services;
        }

        //falla al arrancar, y no en el primer login, si faltan secretos; el mensaje nombra la clave, nunca su valor
        private static void ValidarConfiguracion(IConfiguration configuration)
        {
            const int BytesMinimosHmacSha256 = 32;

            if (string.IsNullOrWhiteSpace(configuration.GetConnectionString("connection")))
                throw new InvalidOperationException(
                    "Falta la cadena de conexión 'ConnectionStrings:connection'. Defínala con user-secrets o con la variable de entorno ConnectionStrings__connection.");

            var jwtKey = configuration["Jwt:key"];
            if (string.IsNullOrWhiteSpace(jwtKey) || System.Text.Encoding.UTF8.GetByteCount(jwtKey) < BytesMinimosHmacSha256)
                throw new InvalidOperationException(
                    $"Falta 'Jwt:key' o tiene menos de {BytesMinimosHmacSha256} bytes. Defínala con user-secrets o con la variable de entorno Jwt__key.");
        }
    }
}
