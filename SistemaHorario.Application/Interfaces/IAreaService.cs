using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IAreaService
    {
        Task<List<AreaDTO>> GetAll();
        Task<List<AreaDTO>> GetAreasWithTotalHrs(string ciclo);
    }
}
