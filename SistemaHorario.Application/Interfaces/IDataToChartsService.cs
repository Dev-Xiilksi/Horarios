using SistemaHorario.Application.DTOs.Auxiliares;


namespace SistemaHorario.Application.Interfaces
{
    public interface IDataToChartsService
    {
        //To working with charts
        Task<HorarioAuxDTO> GetHour(string employee);
        Task<List<HorarioAuxDTO>> OnlyHours(string employee);

        //To show in Dashboard Home
        Task<List<HorarioAuxDTO>> OnlyHoursByArea(string area);
    }
}
