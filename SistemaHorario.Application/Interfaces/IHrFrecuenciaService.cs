using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrFrecuenciaService
    {
        Task<List<HrFrecuenciaDTO>> GetAll();
    }
}
