using MediatR;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.UseCases.Horario.Commands
{
    //aprueba un horario; quien aprueba es el usuario de la sesion
    public record AprobarHorarioCommand(int IdHorario) : IRequest<bool>;

    public class AprobarHorarioHandler : IRequestHandler<AprobarHorarioCommand, bool>
    {
        private readonly IHorarioRepository _repository;
        private readonly IUsuarioActual _usuarioActual;
        private readonly IHrBitacoraService _bitacora;

        public AprobarHorarioHandler(IHorarioRepository repository, IUsuarioActual usuarioActual, IHrBitacoraService bitacora)
        {
            _repository = repository;
            _usuarioActual = usuarioActual;
            _bitacora = bitacora;
        }

        public async Task<bool> Handle(AprobarHorarioCommand command, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);

            //se carga de la base de datos: la pantalla solo indica cual horario
            var horario = await _repository.Buscar(command.IdHorario)
                ?? throw new InvalidOperationException("El horario no existe.");

            if (!ReglasDeAcceso.PuedeAprobar(usuario, horario.Area ?? -1))
                throw new AccesoDenegadoException("No tiene permiso para aprobar horarios de esta área.");

            //aprobar dos veces no duplica la confirmacion
            if (horario.EstaAprobado())
                return true;

            var confirmacion = horario.Aprobar(usuario.IdEmpleado);

            //estado del horario y confirmacion en un solo SaveChanges (una transaccion)
            if (await _repository.GuardarAprobacion(horario, confirmacion) == 0)
                throw new InvalidOperationException("No se pudo guardar la aprobación.");

            await _bitacora.GuardarBitacora(
                usuario.IdEmpleado,
                horario.IdHorario1,
                "Aprobación",
                $"Ha aprobado el horario de ({horario.Empleado}).");

            return true;
        }
    }
}
