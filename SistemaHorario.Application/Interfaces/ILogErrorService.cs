using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface ILogErrorService
    {
        Task GuardarError(Exception ex, string? metodo = null, string? usuario = null);
    }
}
