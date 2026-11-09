using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IColegioNivelGradoSeccionService
    {
        Task<List<ColegioNivelGradoSeccionDTO>> GetAll();
    }
}
