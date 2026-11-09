using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrGeneralService
    {
        Task<List<HrGeneralDTO>> GetAll();
        Task<List<HrGeneralDTO>> GeneralList();
    }
}
