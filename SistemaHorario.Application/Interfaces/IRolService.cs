using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IRolService
    {
        Task<List<RolDTO>> GetAll();
    }
}
