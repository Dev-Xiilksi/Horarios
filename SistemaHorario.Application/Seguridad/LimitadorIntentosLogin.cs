using System.Threading.RateLimiting;

namespace SistemaHorario.Application.Seguridad
{
    public record LimitesLogin(int IntentosPorUsuario, int IntentosPorIp, TimeSpan Ventana)
    {
        //5 intentos fallidos por usuario y 20 por IP cada 15 minutos (ventana deslizante en 3 tramos de 5 minutos)
        public static LimitesLogin PorDefecto => new(5, 20, TimeSpan.FromMinutes(15));
    }

    //Limita los intentos de inicio de sesion antes de consultar el directorio activo.
    //Por usuario frena que adivinen la contrasena de una cuenta (y que bloqueen la cuenta en el AD);
    //por IP frena que prueben una contrasena contra muchos usuarios.
    //Debe registrarse como singleton: el conteo se comparte entre todas las conexiones.
    public sealed class LimitadorIntentosLogin : IDisposable
    {
        private const int Tramos = 3;

        private readonly PartitionedRateLimiter<string> _porUsuario;
        private readonly PartitionedRateLimiter<string> _porIp;
        private readonly TimeSpan _ventana;

        public LimitadorIntentosLogin() : this(LimitesLogin.PorDefecto)
        {
        }

        public LimitadorIntentosLogin(LimitesLogin limites)
        {
            _ventana = limites.Ventana;
            _porUsuario = Crear(limites.IntentosPorUsuario, limites.Ventana);
            _porIp = Crear(limites.IntentosPorIp, limites.Ventana);
        }

        public TimeSpan Ventana => _ventana;

        //Solo cuentan los intentos fallidos (contrasena incorrecta), asi los inicios de sesion correctos
        //de muchos usuarios detras de la misma IP no se bloquean entre si.
        //Sin IP conocida solo se aplica el limite por usuario, para no bloquear a todos con una clave comun.

        //true si quedan intentos para ese usuario y esa IP; no consume ninguno
        public bool PuedeIntentar(string? usuario, string? ip)
        {
            //pedir 0 permisos solo consulta si hay disponibles
            using var porUsuario = _porUsuario.AttemptAcquire(NormalizarUsuario(usuario), 0);
            if (!porUsuario.IsAcquired)
                return false;

            if (string.IsNullOrWhiteSpace(ip))
                return true;

            using var porIp = _porIp.AttemptAcquire(ip, 0);
            return porIp.IsAcquired;
        }

        public void RegistrarFallo(string? usuario, string? ip)
        {
            _porUsuario.AttemptAcquire(NormalizarUsuario(usuario)).Dispose();

            if (!string.IsNullOrWhiteSpace(ip))
                _porIp.AttemptAcquire(ip).Dispose();
        }

        //"DOMINIO\Usuario", "usuario@dominio" y "USUARIO" cuentan como el mismo usuario
        public static string NormalizarUsuario(string? usuario)
        {
            var u = (usuario ?? string.Empty).Trim().ToLowerInvariant();
            var barra = u.LastIndexOf('\\');
            if (barra >= 0)
                u = u[(barra + 1)..];
            var arroba = u.IndexOf('@');
            if (arroba >= 0)
                u = u[..arroba];
            return u;
        }

        private static PartitionedRateLimiter<string> Crear(int permitidos, TimeSpan ventana) =>
            PartitionedRateLimiter.Create<string, string>(clave =>
                RateLimitPartition.GetSlidingWindowLimiter(clave, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = permitidos,
                    Window = ventana,
                    SegmentsPerWindow = Tramos,
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

        public void Dispose()
        {
            _porUsuario.Dispose();
            _porIp.Dispose();
        }
    }
}
