using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.RateLimiting;
using SistemaHorarioApp.Seguridad;

namespace SistemaHorarioApp.Pages.Account
{
    //Cerrar sesion solo por POST con token antifalsificacion (lo valida Razor Pages automaticamente)
    [AllowAnonymous]
    [EnableRateLimiting(PoliticasLimite.Cuenta)]
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet() => Redirect("/");

        public async Task<IActionResult> OnPost()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/");
        }
    }
}
