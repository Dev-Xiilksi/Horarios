using Microsoft.Extensions.Logging.Abstractions;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Application.Services;
using SistemaHorario.Application.UseCases.Horario.Commands;
using SistemaHorario.Application.UseCases.Horario.Queries;
using SistemaHorario.Domain.Constants;
using SistemaHorario.Domain.Entities;
using SistemaHorario.Tests.Fakes;

namespace SistemaHorario.Tests
{
    public class HorarioHandlersTests
    {
        private readonly HorarioRepositoryFalso _repo = new();
        private readonly HorarioConsultasFalsas _consultas = new();
        private readonly BitacoraFalsa _bitacora = new();

        public HorarioHandlersTests()
        {
            //horario 10: empleado 200 del area 3, pendiente
            _repo.Horarios[10] = new HrHorario1 { IdHorario1 = 10, Empleado = 200, Area = 3, IdConfirmacion = EstadoConfirmacion.Pendiente };
            _consultas.Horarios.Add(new HrHorario1DTO { IdHorario1 = 10, Empleado = 200, Area = 3 });
            _consultas.Horarios.Add(new HrHorario1DTO { IdHorario1 = 11, Empleado = 300, Area = 5 });
            _consultas.Horarios.Add(new HrHorario1DTO { IdHorario1 = 12, Empleado = 100, Area = 3 });
        }

        private AprobarHorarioHandler Aprobar(UsuarioActual? u) =>
            new(_repo, new UsuarioFijo(u), _bitacora);

        [Fact]
        public async Task Coordinador_aprueba_en_su_area_y_queda_registrado_como_quien_aprueba()
        {
            var ok = await Aprobar(Usuarios.CoordinadorArea3).Handle(new AprobarHorarioCommand(10), default);

            Assert.True(ok);
            Assert.Equal(EstadoConfirmacion.Aprobado, _repo.Horarios[10].IdConfirmacion);
            Assert.Equal(Usuarios.CoordinadorArea3.IdEmpleado, Assert.Single(_repo.Confirmaciones).id_coordinador);
            Assert.Equal(Usuarios.CoordinadorArea3.IdEmpleado, Assert.Single(_bitacora.Registros).Usuario);
        }

        [Fact]
        public async Task Coordinador_de_otra_area_no_puede_aprobar()
        {
            _repo.Horarios[10].Area = 5;

            await Assert.ThrowsAsync<AccesoDenegadoException>(() => Aprobar(Usuarios.CoordinadorArea3).Handle(new AprobarHorarioCommand(10), default));
            Assert.Empty(_repo.Confirmaciones);
        }

        [Fact]
        public async Task Educador_no_puede_aprobar()
        {
            await Assert.ThrowsAsync<AccesoDenegadoException>(() => Aprobar(Usuarios.Educador100).Handle(new AprobarHorarioCommand(10), default));
        }

        [Fact]
        public async Task Sin_sesion_no_puede_aprobar()
        {
            await Assert.ThrowsAsync<AccesoDenegadoException>(() => Aprobar(null).Handle(new AprobarHorarioCommand(10), default));
        }

        [Fact]
        public async Task Aprobar_dos_veces_no_duplica_la_confirmacion()
        {
            await Aprobar(Usuarios.Administrador).Handle(new AprobarHorarioCommand(10), default);
            await Aprobar(Usuarios.Administrador).Handle(new AprobarHorarioCommand(10), default);

            Assert.Single(_repo.Confirmaciones);
        }

        private EliminarHorarioHandler Eliminar(UsuarioActual u) =>
            new(_repo, new UsuarioFijo(u), _bitacora, new LogErrorFalso(), NullLogger<EliminarHorarioHandler>.Instance);

        [Fact]
        public async Task Educador_elimina_su_horario()
        {
            var r = await Eliminar(Usuarios.Educador100).Handle(new EliminarHorarioCommand(100), default);

            Assert.True(r.status);
            Assert.True(_repo.Eliminado);
        }

        [Fact]
        public async Task Educador_no_elimina_el_horario_de_otro()
        {
            var r = await Eliminar(Usuarios.Educador100).Handle(new EliminarHorarioCommand(200), default);

            Assert.False(r.status);
            Assert.False(_repo.Eliminado);
        }

        [Fact]
        public async Task No_se_elimina_un_horario_aprobado()
        {
            _repo.HayAprobado = true;
            var r = await Eliminar(Usuarios.Educador100).Handle(new EliminarHorarioCommand(100), default);

            Assert.False(r.status);
            Assert.False(_repo.Eliminado);
        }

        [Fact]
        public async Task Educador_no_crea_el_horario_de_otro()
        {
            var handler = new CrearHorarioHandler(_repo, _consultas, new UsuarioFijo(Usuarios.Educador100), _bitacora);

            await Assert.ThrowsAsync<AccesoDenegadoException>(() => handler.Handle(new CrearHorarioCommand(new HrHorario1DTO { Empleado = 200 }), default));
            Assert.Null(_repo.Agregado);
        }

        private ObtenerHorariosHandler Listar(UsuarioActual u) =>
            new(_consultas, new UsuarioFijo(u));

        [Fact]
        public async Task La_lista_de_horarios_se_filtra_por_rol()
        {
            Assert.Equal(3, (await Listar(Usuarios.Administrador).Handle(new ObtenerHorariosQuery("2026"), default)).Count);
            Assert.Equal(3, (await Listar(Usuarios.CoordinadorGlobal).Handle(new ObtenerHorariosQuery("2026"), default)).Count);
            Assert.Equal(new[] { 10, 12 }, (await Listar(Usuarios.CoordinadorArea3).Handle(new ObtenerHorariosQuery("2026"), default)).Select(h => h.IdHorario1));
            Assert.Equal(new[] { 12 }, (await Listar(Usuarios.Educador100).Handle(new ObtenerHorariosQuery("2026"), default)).Select(h => h.IdHorario1));
        }

        [Fact]
        public async Task Educador_no_ve_el_horario_ni_las_franjas_de_otro()
        {
            var u = new UsuarioFijo(Usuarios.Educador100);
            var porEmpleado = new ObtenerHorarioPorEmpleadoHandler(_consultas, u);
            var franjas = new ObtenerFranjasDelHorarioHandler(_consultas, u);

            await Assert.ThrowsAsync<AccesoDenegadoException>(() => porEmpleado.Handle(new ObtenerHorarioPorEmpleadoQuery("200", "2026"), default));
            await Assert.ThrowsAsync<AccesoDenegadoException>(() => franjas.Handle(new ObtenerFranjasDelHorarioQuery(10), default));
            Assert.Single(await franjas.Handle(new ObtenerFranjasDelHorarioQuery(12), default));
        }

        [Fact]
        public async Task Solo_el_administrador_modifica_accesos()
        {
            var repo = new AccesoUsuarioRepositoryFalso();
            var comoCoordinador = new AccesoUsuarioService(repo, NullLogger<AccesoUsuarioService>.Instance, new UsuarioFijo(Usuarios.CoordinadorGlobal));

            await Assert.ThrowsAsync<AccesoDenegadoException>(() => comoCoordinador.Update(new AccesoUsuarioDTO { IdAcceso = 1, IdRol = 1 }));
            await Assert.ThrowsAsync<AccesoDenegadoException>(() => comoCoordinador.Delete(100));
            Assert.False(repo.Modificado);
            Assert.Single(await comoCoordinador.GetAll());

            var comoAdmin = new AccesoUsuarioService(repo, NullLogger<AccesoUsuarioService>.Instance, new UsuarioFijo(Usuarios.Administrador));
            Assert.True(await comoAdmin.Update(new AccesoUsuarioDTO { IdAcceso = 1, IdRol = 1 }));
        }
    }
}
