using Microsoft.Extensions.Configuration;
using SistemaHorario.Application.Interfaces;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

namespace SistemaHorario.Infrastructure.Security
{
    public class DirectorioActivoService : IDirectorioActivoService
    {
        private readonly string _domain;

        public DirectorioActivoService(IConfiguration configuration)
        {
            _domain = configuration["ActiveDirectory:Domain"]!;
        }

        public ResultadoDirectorio ValidarCredenciales(string? userName, string? password)
        {
            using PrincipalContext context = new PrincipalContext(ContextType.Domain, _domain);

            //valida las credenciales
            if (!context.ValidateCredentials(userName, password))
                return new ResultadoDirectorio(false, null);

            //valida el usuario
            UserPrincipal user = UserPrincipal.FindByIdentity(context, userName);
            if (user == null)
                return new ResultadoDirectorio(true, null);

            if (user.GetUnderlyingObject() is not DirectoryEntry de)
                return new ResultadoDirectorio(true, null);

            //el codigo de empleado se guarda en el campo department del AD
            string idUser = de.Properties["department"].Value!.ToString()!;
            return new ResultadoDirectorio(true, new UsuarioDirectorio(idUser, user.SamAccountName!));
        }
    }
}
