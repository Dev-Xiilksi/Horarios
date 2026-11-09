using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.UseCases.Horario.Queries
{
    //horario del empleado en el ciclo; null si no tiene
    public record ObtenerHorarioPorEmpleadoQuery(string Empleado, string Ciclo) : IRequest<HrHorario1DTO>;

    public class ObtenerHorarioPorEmpleadoHandler : IRequestHandler<ObtenerHorarioPorEmpleadoQuery, HrHorario1DTO>
    {
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerHorarioPorEmpleadoHandler(IHorarioConsultas consultas, IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _usuarioActual = usuarioActual;
        }

        public async Task<HrHorario1DTO> Handle(ObtenerHorarioPorEmpleadoQuery query, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var horario = await _consultas.ObtenerPorEmpleado(Convert.ToInt32(query.Empleado), query.Ciclo);

            if (horario != null && !ReglasDeAcceso.PuedeVerHorario(usuario, horario.Empleado, horario.Area))
                throw new AccesoDenegadoException("No tiene permiso para ver este horario.");

            return horario!;
        }
    }
}
