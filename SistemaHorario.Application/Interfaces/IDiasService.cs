using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IDiasService
    {
        Task<List<DiaDTO>> GetAll(); 
    }
}
