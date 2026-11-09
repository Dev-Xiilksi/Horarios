namespace SistemaHorario.Application.Interfaces
{
    //datos de la sesion del usuario que hace la peticion; nunca vienen de la pantalla
    public record UsuarioActual(int IdEmpleado, string Rol, short IdArea);

    public interface IUsuarioActual
    {
        //null si no hay sesion valida
        Task<UsuarioActual?> ObtenerAsync();
    }
}
