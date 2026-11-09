using Microsoft.AspNetCore.Components.Authorization;
using SistemaHorario.Application.Interfaces;
using System.Security.Claims;

namespace SistemaHorarioApp.Seguridad
{
    //Lee el usuario de la cookie de sesion. En Blazor Server se usa AuthenticationStateProvider
    //porque HttpContext no esta disponible durante el circuito.
    public class UsuarioActualBlazor : IUsuarioActual
    {
        private readonly AuthenticationStateProvider _auth;

        public UsuarioActualBlazor(AuthenticationStateProvider auth)
        {
            _auth = auth;
        }

        public async Task<UsuarioActual?> ObtenerAsync()
        {
            var user = (await _auth.GetAuthenticationStateAsync()).User;
            if (user.Identity?.IsAuthenticated != true)
                return null;

            if (!int.TryParse(user.FindFirst("idUser")?.Value, out var idEmpleado))
                return null;

            //sin area valida no coincide con ningun area real
            if (!short.TryParse(user.FindFirst("idArea")?.Value, out var idArea))
                idArea = -1;

            return new UsuarioActual(idEmpleado, user.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty, idArea);
        }
    }
}
