using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrGradosService
    {
        Task<List<HrGradosDto>> ObtenerGrados(int idhorario1);
    }
}
