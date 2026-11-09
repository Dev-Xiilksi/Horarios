namespace SistemaHorario.Application.Interfaces
{
    //IdEmpleado es el codigo de empleado guardado en el directorio (campo department)
    public record UsuarioDirectorio(string IdEmpleado, string UserName);

    //Usuario es null cuando las credenciales son validas pero no se encontro el usuario
    public record ResultadoDirectorio(bool CredencialesValidas, UsuarioDirectorio? Usuario);

    public interface IDirectorioActivoService
    {
        ResultadoDirectorio ValidarCredenciales(string? userName, string? password);
    }
}
