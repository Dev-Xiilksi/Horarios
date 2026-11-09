using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using SistemaHorarioApp.Seguridad;
using System.Security.Claims;

namespace SistemaHorarioApp.Pages.Account
{
    //Crea la cookie de sesion a partir de un ticket emitido por el componente de login.
    //No acepta datos de usuario por la URL: solo el ticket, que caduca en segundos y se usa una vez.
    [AllowAnonymous]
    [EnableRateLimiting(PoliticasLimite.Cuenta)]
    public class LoginModel : PageModel
    {
        private readonly LoginTicketStore _tickets;

        public LoginModel(LoginTicketStore tickets)
        {
            _tickets = tickets;
        }

        public async Task<IActionResult> OnGet(string? ticket)
        {
            var sesion = _tickets.Canjear(ticket);
            if (sesion == null || !sesion.IsAuthenticated)
                return Redirect("/");

            var claims = new List<Claim>
            {
                new Claim("idUser", sesion.IdUser ?? string.Empty),
                new Claim(ClaimTypes.Name, sesion.UserName ?? string.Empty),
                new Claim("group", sesion.Group ?? string.Empty),
                new Claim(ClaimTypes.Role, sesion.Rol?.Trim() ?? string.Empty),
                new Claim("area", sesion.AreaName ?? "area"),
                new Claim("idArea", sesion.Area.ToString())
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return Redirect("/page/home");
        }
    }
}
