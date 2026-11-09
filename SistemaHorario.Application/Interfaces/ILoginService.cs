using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface ILoginService
    {
        //ipCliente: direccion del navegador, para limitar intentos por IP; null si no se conoce
        Task<LoginDTO> Login(LoginRequestDTO model, string? ipCliente = null);
    }
}
