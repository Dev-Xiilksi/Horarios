using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.UseCases.Horario.Queries
{
    public record ObtenerHorarioPorIdQuery(int IdHorario) : IRequest<HrHorario1DTO?>;

    public class ObtenerHorarioPorIdHandler : IRequestHandler<ObtenerHorarioPorIdQuery, HrHorario1DTO?>
    {
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerHorarioPorIdHandler(IHorarioConsultas consultas, IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _usuarioActual = usuarioActual;
        }

        public async Task<HrHorario1DTO?> Handle(ObtenerHorarioPorIdQuery query, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var horario = await _consultas.ObtenerPorId(query.IdHorario);

            if (horario != null && !ReglasDeAcceso.PuedeVerHorario(usuario, horario.Empleado, horario.Area))
                throw new AccesoDenegadoException("No tiene permiso para ver este horario.");

            return horario;
        }
    }
}
