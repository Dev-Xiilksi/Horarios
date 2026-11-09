using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IEmailService
    {
        Task EnviarCorreo(string destino, string asunto, string mensaje);
        Task RegistrarCorreo(CorreoDTO dto);
        Task ObtenerCorreos(CorreoDTO dto);
    }
}
