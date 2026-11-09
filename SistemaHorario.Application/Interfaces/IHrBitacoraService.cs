using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IHrBitacoraService
    {
        Task GuardarBitacora(int? usuario, int horario, string? accion, string? mensaje);
    }
}
