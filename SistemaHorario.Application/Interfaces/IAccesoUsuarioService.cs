using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IAccesoUsuarioService
    {
        Task<List<AccesoUsuarioDTO>> GetAll();
        Task<AccesoUsuarioDTO> GetById(int educador);
        Task<AccesoUsuarioDTO> Create(AccesoUsuarioDTO model);
        Task<bool> Update(AccesoUsuarioDTO model);
        Task<bool> Delete(int id);
    }
}
