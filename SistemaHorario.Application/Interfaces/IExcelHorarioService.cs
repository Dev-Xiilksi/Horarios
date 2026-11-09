using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IExcelHorarioService
    {
        byte[] GenerarExcel(HorarioExcelDto horario);
        byte[] GenerarExcelConsolidados(IEnumerable<HorarioConsolidadoDto> horario);
        byte[] GenerarExcelResumen(IEnumerable<HorarioAuxDTO> horarios);
    }
}
