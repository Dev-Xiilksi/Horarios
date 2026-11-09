using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.UseCases.Horario.Queries
{
    //franjas (hr_horario2) de un horario
    public record ObtenerFranjasDelHorarioQuery(int IdHorario) : IRequest<List<HrHorario2DTO>>;

    public class ObtenerFranjasDelHorarioHandler : IRequestHandler<ObtenerFranjasDelHorarioQuery, List<HrHorario2DTO>>
    {
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerFranjasDelHorarioHandler(IHorarioConsultas consultas, IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _usuarioActual = usuarioActual;
        }

        public async Task<List<HrHorario2DTO>> Handle(ObtenerFranjasDelHorarioQuery query, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            //el permiso depende de a quien pertenece el horario
            var horario = await _consultas.ObtenerPorId(query.IdHorario);
            if (horario == null)
                return new List<HrHorario2DTO>();

            if (!ReglasDeAcceso.PuedeVerHorario(usuario, horario.Empleado, horario.Area))
                throw new AccesoDenegadoException("No tiene permiso para ver este horario.");

            return await _consultas.ObtenerFranjas(query.IdHorario);
        }
    }
}
