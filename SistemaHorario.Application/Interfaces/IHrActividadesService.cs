using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrActividadesService
    {
        Task<List<ActividadDTO>> GetAll(int empleado);
        Task<List<ActividadDTO>> GetAllVariableActivities();
    }
}
