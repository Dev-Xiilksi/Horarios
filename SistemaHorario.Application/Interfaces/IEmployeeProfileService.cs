using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IEmployeeProfileService
    {
        Task<EmployeeProfileDTO> Get(string employee);
    }
}
