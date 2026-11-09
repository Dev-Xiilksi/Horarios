using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.DirectoryServices.AccountManagement;

namespace SistemaHorario.Infrastructure.Salud
{
    //comprueba que se puede contactar al dominio que valida los inicios de sesion (sin credenciales de usuario)
    public class DirectorioActivoHealthCheck : IHealthCheck
    {
        private readonly string? _domain;

        public DirectorioActivoHealthCheck(IConfiguration configuration)
        {
            _domain = configuration["ActiveDirectory:Domain"];
        }

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                using var dominio = new PrincipalContext(ContextType.Domain, _domain);
                //leer el servidor conectado obliga a contactar un controlador de dominio
                return Task.FromResult(string.IsNullOrEmpty(dominio.ConnectedServer)
                    ? HealthCheckResult.Unhealthy("No se encontró un controlador de dominio.")
                    : HealthCheckResult.Healthy());
            }
            catch (Exception ex)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy("No se pudo contactar el directorio activo.", ex));
            }
        }
    }
}
