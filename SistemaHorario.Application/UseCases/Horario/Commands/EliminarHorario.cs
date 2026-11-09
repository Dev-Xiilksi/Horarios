using Microsoft.Extensions.Logging;
using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Domain.Constants;
using SistemaHorario.Domain.ValueObjects;

namespace SistemaHorario.Application.UseCases.Horario.Commands
{
    //elimina el horario del empleado en el ciclo actual
    public record EliminarHorarioCommand(int Empleado) : IRequest<ResponseDTO<bool>>;

    public class EliminarHorarioHandler : IRequestHandler<EliminarHorarioCommand, ResponseDTO<bool>>
    {
        private readonly IHorarioRepository _repository;
        private readonly IUsuarioActual _usuarioActual;
        private readonly IHrBitacoraService _bitacora;
        private readonly ILogErrorService _logError;
        private readonly ILogger<EliminarHorarioHandler> _logger;

        public EliminarHorarioHandler(IHorarioRepository repository, IUsuarioActual usuarioActual, IHrBitacoraService bitacora, ILogErrorService logError, ILogger<EliminarHorarioHandler> logger)
        {
            _repository = repository;
            _usuarioActual = usuarioActual;
            _bitacora = bitacora;
            _logError = logError;
            _logger = logger;
        }

        public async Task<ResponseDTO<bool>> Handle(EliminarHorarioCommand command, CancellationToken cancellationToken)
        {
            int id = command.Empleado;
            string ciclo = CicloEscolar.Actual();

            var usuario = await _usuarioActual.ObtenerAsync();
            if (usuario == null || !ReglasDeAcceso.PuedeModificarHorarioDe(usuario, id))
            {
                return new ResponseDTO<bool>
                {
                    status = false,
                    value = false,
                    message = "No tiene permiso para eliminar este horario."
                };
            }

            try
            {
                //un horario aprobado ya no se puede eliminar
                if (await _repository.ExisteEnEstado(id, ciclo, EstadoConfirmacion.Aprobado))
                {
                    return new ResponseDTO<bool>
                    {
                        status = false,
                        value = false,
                        message = "El horario ya está aprobado."
                    };
                }

                var idHorario = await _repository.EliminarConDetalle(id, ciclo);

                if (idHorario != null)
                {
                    await _bitacora.GuardarBitacora(
                        usuario.IdEmpleado,
                        idHorario.Value,
                        "Eliminación",
                        $"Ha eliminado registro de horario ({idHorario}).");
                }
                return new ResponseDTO<bool>
                {
                    status = true,
                    value = true,
                    message = ""
                };
            }
            catch (Exception ex)
            {
                await _logError.GuardarError(ex, "EliminarHorarioHandler", id.ToString());
                _logger.LogError(ex, "Error al elmininar el horario.");
                return new ResponseDTO<bool>
                {
                    status = false,
                    value = false,
                    message = ex.Message
                };
            }
        }
    }
}
