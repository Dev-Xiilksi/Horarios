using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IReporteRepository
    {
        Task<List<HorarioAuxDTO>> ObtenerDetalleParaExportar(string? ciclo, int area);
        Task<List<HorarioAuxDTO>> ObtenerHorariosResumen(string ciclo);
        //area null = todas las areas
        Task<List<HorarioConsolidadoDto>> ObtenerConsolidados(string ciclo, string? area);
        Task<ColaboradorDTO?> ObtenerColaborador(int empleado, string ciclo);
        Task<List<HrGradosDto>> ObtenerGrados(int idHorario1);
        Task<Dictionary<int, string>> ObtenerMateriasDelMaestro(int codigoMaestro, string ciclo);
        Task<Dictionary<short, string>> ObtenerActividades();
        Task<List<FranjaHorariaDto>> ObtenerFranjas(int idHorario1);
    }
}
