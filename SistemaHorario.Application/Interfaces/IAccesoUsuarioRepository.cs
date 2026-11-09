using SistemaHorario.Application.DTOs;
using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Application.Interfaces
{
    public interface IAccesoUsuarioRepository
    {
        Task<List<AccesoUsuarioDTO>> ObtenerTodos();
        Task<AccesoUsuarioDTO?> ObtenerPorEmpleado(int idEmpleado);
        Task<AccesoUsuarioDTO?> ObtenerPorId(int idAcceso);
        Task<AccesoUsuario?> Buscar(int idAcceso);
        Task<AccesoUsuario?> BuscarPorEmpleado(int idEmpleado);
        Task Agregar(AccesoUsuario acceso);
        Task<int> Actualizar(AccesoUsuario acceso);
        Task Eliminar(AccesoUsuario acceso);
    }
}
