using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    public interface IUsuarioRepository
    {
        //nombre del rol asignado en acceso_usuario, null si no tiene acceso registrado
        Task<string?> ObtenerRolDeAcceso(int idEmpleado);
        Task<bool> EsMaestro(int idEmpleado);
        //datos del usuario para la sesion (nombre, rol y area)
        Task<LoginDTO?> ObtenerDatosSesion(int idEmpleado);
    }
}
