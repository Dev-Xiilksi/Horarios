using Microsoft.Extensions.Caching.Memory;
using SistemaHorario.Application.DTOs;
using System.Security.Cryptography;

namespace SistemaHorarioApp.Seguridad
{
    //El componente de login (Blazor interactivo) no puede escribir la cookie porque ya no hay peticion HTTP.
    //Guarda aqui la sesion validada y redirige a /Account/Login con un ticket aleatorio, de un solo uso y corta vida;
    //esa pagina lo canjea y crea la cookie. Nada de la sesion viaja en la URL.
    public class LoginTicketStore
    {
        private static readonly TimeSpan Vigencia = TimeSpan.FromSeconds(30);
        private readonly IMemoryCache _cache;

        public LoginTicketStore(IMemoryCache cache)
        {
            _cache = cache;
        }

        public string Emitir(LoginDTO sesion)
        {
            var ticket = Convert.ToHexString(RandomNumberGenerator.GetBytes(32));
            _cache.Set(Clave(ticket), sesion, Vigencia);
            return ticket;
        }

        public LoginDTO? Canjear(string? ticket)
        {
            if (string.IsNullOrWhiteSpace(ticket))
                return null;

            var clave = Clave(ticket);
            if (!_cache.TryGetValue(clave, out LoginDTO? sesion))
                return null;

            _cache.Remove(clave);
            return sesion;
        }

        private static string Clave(string ticket) => $"login-ticket:{ticket}";
    }
}
