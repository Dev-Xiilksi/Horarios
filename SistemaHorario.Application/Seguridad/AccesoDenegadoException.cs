using SistemaHorario.Application.Interfaces;

namespace SistemaHorario.Application.Seguridad
{
    //el usuario no tiene permiso para la operacion; el mensaje se puede mostrar en pantalla
    public class AccesoDenegadoException : Exception
    {
        public AccesoDenegadoException(string mensaje = "No tiene permiso para realizar esta acción.")
            : base(mensaje)
        {
        }

        public static async Task<UsuarioActual> ExigirSesion(IUsuarioActual usuarioActual) =>
            await usuarioActual.ObtenerAsync()
            ?? throw new AccesoDenegadoException("La sesión no es válida. Inicie sesión de nuevo.");
    }
}
