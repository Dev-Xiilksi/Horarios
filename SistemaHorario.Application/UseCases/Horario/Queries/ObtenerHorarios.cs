using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.UseCases.Horario.Queries
{
    //horarios del ciclo que el usuario puede ver segun su rol y area
    public record ObtenerHorariosQuery(string Ciclo) : IRequest<List<HrHorario1DTO>>;

    public class ObtenerHorariosHandler : IRequestHandler<ObtenerHorariosQuery, List<HrHorario1DTO>>
    {
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerHorariosHandler(IHorarioConsultas consultas, IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _usuarioActual = usuarioActual;
        }

        public async Task<List<HrHorario1DTO>> Handle(ObtenerHorariosQuery query, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var horarios = await _consultas.ObtenerTodos(query.Ciclo);
            return ReglasDeAcceso.FiltrarVisibles(usuario, horarios);
        }
    }
}
