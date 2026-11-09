namespace SistemaHorario.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {
        string GenerateJWT(string IdUser, string userName, string group, string? rol);
        bool TokenValidate(string token);
    }
}
