using Microsoft.Extensions.Logging.Abstractions;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Application.Services;
using SistemaHorario.Tests.Fakes;

namespace SistemaHorario.Tests
{
    public class LimiteLoginTests
    {
        private static LimitadorIntentosLogin Limitador(int porUsuario = 3, int porIp = 5) =>
            new(new LimitesLogin(porUsuario, porIp, TimeSpan.FromMinutes(15)));

        [Fact]
        public void Bloquea_al_superar_los_intentos_del_usuario()
        {
            using var l = Limitador();

            for (var i = 1; i <= 3; i++)
            {
                Assert.True(l.PuedeIntentar("jperez", $"10.0.0.{i}"));
                l.RegistrarFallo("jperez", $"10.0.0.{i}");
            }

            Assert.False(l.PuedeIntentar("jperez", "10.0.0.4"));
        }

        [Fact]
        public void Otro_usuario_no_queda_bloqueado()
        {
            using var l = Limitador();
            for (var i = 0; i < 3; i++) l.RegistrarFallo("jperez", null);

            Assert.False(l.PuedeIntentar("jperez", null));
            Assert.True(l.PuedeIntentar("mlopez", null));
        }

        [Fact]
        public void Bloquea_al_superar_los_intentos_de_una_IP_con_usuarios_distintos()
        {
            using var l = Limitador(porIp: 5);
            for (var i = 0; i < 5; i++)
                l.RegistrarFallo($"usuario{i}", "10.0.0.9");

            Assert.False(l.PuedeIntentar("otro", "10.0.0.9"));
            Assert.True(l.PuedeIntentar("otro", "10.0.0.10"));
        }

        [Theory]
        [InlineData("JPerez")]
        [InlineData(" jperez ")]
        [InlineData(@"LICEO\jperez")]
        [InlineData("jperez@liceojavier.edu.gt")]
        public void Las_variantes_del_nombre_cuentan_como_el_mismo_usuario(string variante)
        {
            Assert.Equal("jperez", LimitadorIntentosLogin.NormalizarUsuario(variante));
        }

        [Fact]
        public async Task El_login_bloqueado_no_consulta_el_directorio()
        {
            var directorio = new DirectorioQueCuenta();
            var servicio = new LoginService(directorio, new UsuarioRepositorySinAcceso(), new JwtFalso(), new LogErrorFalso(),
                Limitador(porUsuario: 2), NullLogger<LoginService>.Instance);
            var intento = new LoginRequestDTO { UserName = "jperez", Password = "mala" };

            await servicio.Login(intento, "10.0.0.1");
            await servicio.Login(intento, "10.0.0.1");
            var bloqueado = await servicio.Login(intento, "10.0.0.1");

            Assert.False(bloqueado.IsAuthenticated);
            Assert.StartsWith("Demasiados intentos", bloqueado.Message);
            Assert.Equal(2, directorio.Consultas);
        }

        [Fact]
        public async Task Los_inicios_de_sesion_correctos_no_gastan_intentos()
        {
            var limitador = Limitador(porUsuario: 2, porIp: 2);
            var directorio = new DirectorioQueCuenta { CredencialesValidas = true };
            var servicio = new LoginService(directorio, new UsuarioRepositorySinAcceso(), new JwtFalso(), new LogErrorFalso(),
                limitador, NullLogger<LoginService>.Instance);

            //muchos usuarios correctos desde la misma IP, como una red con NAT o proxy
            for (var i = 0; i < 10; i++)
                await servicio.Login(new LoginRequestDTO { UserName = $"docente{i}", Password = "buena" }, "10.0.0.1");

            Assert.Equal(10, directorio.Consultas);
            Assert.True(limitador.PuedeIntentar("docente0", "10.0.0.1"));
        }

        private class DirectorioQueCuenta : IDirectorioActivoService
        {
            public int Consultas { get; private set; }
            public bool CredencialesValidas { get; init; }

            public ResultadoDirectorio ValidarCredenciales(string? userName, string? password)
            {
                Consultas++;
                return new ResultadoDirectorio(CredencialesValidas, null);
            }
        }

        private class UsuarioRepositorySinAcceso : IUsuarioRepository
        {
            public Task<string?> ObtenerRolDeAcceso(int idEmpleado) => Task.FromResult<string?>(null);
            public Task<bool> EsMaestro(int idEmpleado) => Task.FromResult(false);
            public Task<LoginDTO?> ObtenerDatosSesion(int idEmpleado) => Task.FromResult<LoginDTO?>(null);
        }

        private class JwtFalso : IJwtTokenGenerator
        {
            public string GenerateJWT(string IdUser, string userName, string group, string? rol) => "token";
            public bool TokenValidate(string token) => true;
        }
    }
}
