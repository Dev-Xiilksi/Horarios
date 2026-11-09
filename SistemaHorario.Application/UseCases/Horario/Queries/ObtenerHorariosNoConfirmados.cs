using MediatR;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Domain.Constants;

namespace SistemaHorario.Application.UseCases.Horario.Queries
{
    //horarios del area y ciclo que no han sido confirmados
    public record ObtenerHorariosNoConfirmadosQuery(FiltroParaReportesDTO Filtro) : IRequest<List<HrHorario1DTO>>;

    public class ObtenerHorariosNoConfirmadosHandler : IRequestHandler<ObtenerHorariosNoConfirmadosQuery, List<HrHorario1DTO>>
    {
        private readonly IHorarioConsultas _consultas;
        private readonly IUsuarioActual _usuarioActual;

        public ObtenerHorariosNoConfirmadosHandler(IHorarioConsultas consultas, IUsuarioActual usuarioActual)
        {
            _consultas = consultas;
            _usuarioActual = usuarioActual;
        }

        public async Task<List<HrHorario1DTO>> Handle(ObtenerHorariosNoConfirmadosQuery query, CancellationToken cancellationToken)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var area = Convert.ToInt16(query.Filtro.Area);
            if (!ReglasDeAcceso.PuedeVerArea(usuario, area))
                throw new AccesoDenegadoException("No tiene permiso para ver los horarios de esta área.");

            return await _consultas.ObtenerPorEstado(area, query.Filtro.Ciclo, EstadoConfirmacion.NoConfirmado);
        }
    }
}
