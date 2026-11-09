using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrVariableService
    {
        Task<List<HrVariableDTO>> GetAll();
    }
}
