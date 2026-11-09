using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IGradosService
    {
        Task<List<GradoDTO>> GetAll();
        Task<List<GradoDTO>> GetAllFilter();
        Task<List<HrGradosDto>> ObtenerGrados(int idhorario1);
    }
}
