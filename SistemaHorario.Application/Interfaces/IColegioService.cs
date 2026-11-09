using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IColegioService
    {
        Task<List<ColegioDTO>> GetAll();
    }
}
