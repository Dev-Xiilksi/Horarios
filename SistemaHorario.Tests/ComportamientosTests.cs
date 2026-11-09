using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SistemaHorario.Application;
using SistemaHorario.Application.Comportamientos;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Application.UseCases.Horario.Commands;
using SistemaHorario.Domain.Constants;
using SistemaHorario.Tests.Fakes;

namespace SistemaHorario.Tests
{
    //prueban los behaviors pasando por MediatR, con el mismo registro que usa la app (AddApplication)
    public class ComportamientosTests
    {
        private readonly HorarioRepositoryFalso _repo = new();
        private readonly LogErrorFalso _logError = new();

        private ISender Sender(UsuarioActual usuario)
        {
            var services = new ServiceCollection();
            services.AddSingleton<ILoggerFactory>(NullLoggerFactory.Instance);
            services.AddSingleton(typeof(ILogger<>), typeof(NullLogger<>));
            services.AddApplication();
            services.AddSingleton<IUsuarioActual>(new UsuarioFijo(usuario));
            services.AddSingleton<IHorarioRepository>(_repo);
            services.AddSingleton<IHorarioConsultas>(new HorarioConsultasFalsas());
            services.AddSingleton<IHrBitacoraService>(new BitacoraFalsa());
            services.AddSingleton<ILogErrorService>(_logError);
            return services.BuildServiceProvider().CreateScope().ServiceProvider.GetRequiredService<ISender>();
        }

        private static HrHorario1DTO HorarioValido() => new()
        {
            Empleado = 100,
            Ciclo = "2026",
            TipoFranja = TipoFranja.Fijo,
            Grados = { "P1" },
            HrHorario2s = { new HrHorario2DTO { NumeroFila = 1, Horai = "07:00:00", Horaf = "07:45:00" } }
        };

        [Fact]
        public async Task Hora_final_anterior_a_la_inicial_se_rechaza_con_mensaje_claro()
        {
            var horario = HorarioValido();
            horario.HrHorario2s.First().Horai = "09:00:00";
            horario.HrHorario2s.First().Horaf = "08:00:00";

            var ex = await Assert.ThrowsAsync<DatosInvalidosException>(() => Sender(Usuarios.Educador100).Send(new CrearHorarioCommand(horario)));

            Assert.Contains("Franja 1: la hora final (08:00:00) es anterior a la inicial (09:00:00).", ex.Errores);
            Assert.Null(_repo.Agregado);
            Assert.Empty(_logError.Errores);
        }

        [Fact]
        public async Task Tipo_de_horario_y_ciclo_invalidos_se_rechazan()
        {
            var horario = HorarioValido();
            horario.TipoFranja = 7;
            horario.Ciclo = "";

            var ex = await Assert.ThrowsAsync<DatosInvalidosException>(() => Sender(Usuarios.Educador100).Send(new CrearHorarioCommand(horario)));

            Assert.Equal(2, ex.Errores.Count);
        }

        [Fact]
        public async Task Horario_valido_pasa_la_validacion_y_se_guarda()
        {
            await Sender(Usuarios.Educador100).Send(new CrearHorarioCommand(HorarioValido()));

            Assert.NotNull(_repo.Agregado);
            Assert.Equal("00:45:00", _repo.Agregado!.HrHorario2s.Single().Tiempo);
        }

        [Fact]
        public async Task Error_inesperado_se_registra_con_el_usuario_y_se_relanza()
        {
            //el horario 10 no existe en el repositorio falso
            await Assert.ThrowsAsync<InvalidOperationException>(() => Sender(Usuarios.Administrador).Send(new AprobarHorarioCommand(10)));

            var registro = Assert.Single(_logError.Errores);
            Assert.Equal(nameof(AprobarHorarioCommand), registro.Metodo);
            Assert.Equal(Usuarios.Administrador.IdEmpleado.ToString(), registro.Usuario);
        }

        [Fact]
        public async Task Falta_de_permiso_no_se_registra_como_error()
        {
            var deOtro = HorarioValido();
            deOtro.Empleado = 200;

            await Assert.ThrowsAsync<AccesoDenegadoException>(() => Sender(Usuarios.Educador100).Send(new CrearHorarioCommand(deOtro)));
            Assert.Empty(_logError.Errores);
        }
    }
}
