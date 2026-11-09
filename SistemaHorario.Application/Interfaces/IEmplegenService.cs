using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IEmplegenService
    {
        Task<List<EmplegenDTO>> GetAll();
    }
}
