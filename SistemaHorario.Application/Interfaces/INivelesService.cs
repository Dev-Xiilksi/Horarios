using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface INivelesService
    {
        Task<List<NivelDTO>> GetAll();
    }
}
