using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IMaestrosPorSeccionService
    {
        Task<List<MaestrosporseccionDTO>> GetAll();
    }
}
