using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrGeneral2Service
    {
        Task<List<HrGeneral2DTO>> GetAll();
    }
}
