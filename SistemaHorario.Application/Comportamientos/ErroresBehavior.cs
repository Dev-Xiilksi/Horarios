using MediatR;
using Microsoft.Extensions.Logging;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.Comportamientos
{
    //Envuelve cada comando y consulta: registra los errores inesperados en el log y en hr_log_errors
    //y los vuelve a lanzar. Los permisos y los datos invalidos son respuestas esperadas: no se registran como error.
    public class ErroresBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly ILogErrorService _logError;
        private readonly IUsuarioActual _usuarioActual;
        private readonly ILogger<ErroresBehavior<TRequest, TResponse>> _logger;

        public ErroresBehavior(ILogErrorService logError, IUsuarioActual usuarioActual, ILogger<ErroresBehavior<TRequest, TResponse>> logger)
        {
            _logError = logError;
            _usuarioActual = usuarioActual;
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            try
            {
                return await next(cancellationToken);
            }
            catch (AccesoDenegadoException ex)
            {
                _logger.LogWarning("{Peticion} rechazada por permisos: {Mensaje}", typeof(TRequest).Name, ex.Message);
                throw;
            }
            catch (DatosInvalidosException)
            {
                throw;
            }
            catch (Exception ex)
            {
                var nombre = typeof(TRequest).Name;
                _logger.LogError(ex, "Error al procesar {Peticion}", nombre);
                await RegistrarSinFallar(ex, nombre);
                throw;
            }
        }

        //si el registro de errores falla, no debe ocultar el error original
        private async Task RegistrarSinFallar(Exception ex, string peticion)
        {
            try
            {
                var usuario = await _usuarioActual.ObtenerAsync();
                await _logError.GuardarError(ex, peticion, usuario?.IdEmpleado.ToString() ?? "sin sesión");
            }
            catch (Exception errorAlRegistrar)
            {
                _logger.LogError(errorAlRegistrar, "No se pudo guardar el error de {Peticion} en hr_log_errors", peticion);
            }
        }
    }
}
