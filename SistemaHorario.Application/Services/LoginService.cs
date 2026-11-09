using SistemaHorario.Application.DTOs;
using Microsoft.Extensions.Logging;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;

namespace SistemaHorario.Application.Services
{
    public class LoginService : ILoginService
    {
        private const string Grupo = "group";

        private readonly IDirectorioActivoService _directorio;
        private readonly IUsuarioRepository _usuarios;
        private readonly IJwtTokenGenerator _jwt;
        private readonly ILogErrorService _logError;
        private readonly LimitadorIntentosLogin _limitador;
        private readonly ILogger<LoginService> _logger;

        public LoginService(IDirectorioActivoService directorio, IUsuarioRepository usuarios, IJwtTokenGenerator jwt, ILogErrorService logError, LimitadorIntentosLogin limitador, ILogger<LoginService> logger)
        {
            _directorio = directorio;
            _usuarios = usuarios;
            _jwt = jwt;
            _logError = logError;
            _limitador = limitador;
            _logger = logger;
        }

        public async Task<LoginDTO> Login(LoginRequestDTO model, string? ipCliente = null)
        {
            //se revisa antes de consultar el AD: los intentos bloqueados no llegan al directorio
            if (!_limitador.PuedeIntentar(model.UserName, ipCliente))
            {
                _logger.LogWarning("Login bloqueado por exceso de intentos. Usuario: {Usuario}, IP: {Ip}", LimitadorIntentosLogin.NormalizarUsuario(model.UserName), ipCliente ?? "desconocida");
                return new LoginDTO
                {
                    IsAuthenticated = false,
                    UserName = model.UserName,
                    Message = $"Demasiados intentos de inicio de sesión. Espere {_limitador.Ventana.TotalMinutes:0} minutos e intente de nuevo."
                };
            }

            try
            {
                var resultado = _directorio.ValidarCredenciales(model.UserName, model.Password);

                if (!resultado.CredencialesValidas)
                {
                    //solo los fallos de contrasena cuentan para el limite de intentos
                    _limitador.RegistrarFallo(model.UserName, ipCliente);
                    return new LoginDTO { IsAuthenticated = false, UserName = model.UserName, Message = "Credenciales incorrectas." };
                }

                if (resultado.Usuario == null)
                    return new LoginDTO { IsAuthenticated = false, UserName = model.UserName };

                var usuario = resultado.Usuario;
                int idEmpleado = Convert.ToInt32(usuario.IdEmpleado);

                //solo entra quien tiene acceso registrado o es maestro
                var rol = await _usuarios.ObtenerRolDeAcceso(idEmpleado);
                if (rol == null && !await _usuarios.EsMaestro(idEmpleado))
                {
                    return new LoginDTO
                    {
                        IsAuthenticated = false,
                        UserName = model.UserName,
                        Message = "No tienes permiso para acceder."
                    };
                }

                var sesion = await _usuarios.ObtenerDatosSesion(idEmpleado);
                if (sesion != null)
                {
                    sesion.Group = Grupo;
                    sesion.IsAuthenticated = true;
                    sesion.Token = _jwt.GenerateJWT(usuario.IdEmpleado, usuario.UserName, Grupo, rol!.Trim());
                }
                return sesion!;
            }
            catch (Exception ex)
            {
                await _logError.GuardarError(ex, "LoginService.Login", model.UserName ?? "Not User");
                throw;
            }
        }
    }
}
