using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IReporteService
    {
        Task<byte[]> GenerateExcelWithCustomHeadersAsync(AreaDTO obj);
        Task<List<HorarioAuxDTO>> ObtenerHorariosResumen(string ciclo);
        Task<HorarioExcelDto> ObtenerExcel(int id);
        Task<List<HorarioConsolidadoDto>> ObtenerConsolidadoPorArea(string area, string ciclo);
        Task<List<HorarioConsolidadoDto>> ObtenerConsolidados(string ciclo);
    }
}
