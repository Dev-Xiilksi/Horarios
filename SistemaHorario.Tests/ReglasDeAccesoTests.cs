using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Tests.Fakes;

namespace SistemaHorario.Tests
{
    public class ReglasDeAccesoTests
    {
        public static TheoryData<string, bool> VerHorarioDeOtroEnArea3 => new()
        {
            { "Administrador", true },
            { "Consultor", true },
            { "CoordinadorArea3", true },
            { "CoordinadorGlobal", true },
            { "Educador100", false },
        };

        [Theory]
        [MemberData(nameof(VerHorarioDeOtroEnArea3))]
        public void Ver_horario_de_otro_empleado_del_area_3(string usuario, bool esperado)
        {
            Assert.Equal(esperado, ReglasDeAcceso.PuedeVerHorario(Usuario(usuario), empleado: 200, area: 3));
        }

        [Fact]
        public void Coordinador_no_ve_horarios_de_otra_area()
        {
            Assert.False(ReglasDeAcceso.PuedeVerHorario(Usuarios.CoordinadorArea3, empleado: 200, area: 5));
            Assert.True(ReglasDeAcceso.PuedeVerHorario(Usuarios.CoordinadorGlobal, empleado: 200, area: 5));
        }

        [Fact]
        public void Cualquiera_ve_su_propio_horario()
        {
            Assert.True(ReglasDeAcceso.PuedeVerHorario(Usuarios.Educador100, empleado: 100, area: 9));
        }

        [Fact]
        public void Aprobar_segun_rol_y_area()
        {
            Assert.True(ReglasDeAcceso.PuedeAprobar(Usuarios.Administrador, 5));
            Assert.True(ReglasDeAcceso.PuedeAprobar(Usuarios.Consultor, 5));
            Assert.True(ReglasDeAcceso.PuedeAprobar(Usuarios.CoordinadorArea3, 3));
            Assert.False(ReglasDeAcceso.PuedeAprobar(Usuarios.CoordinadorArea3, 5));
            Assert.True(ReglasDeAcceso.PuedeAprobar(Usuarios.CoordinadorGlobal, 5));
            Assert.False(ReglasDeAcceso.PuedeAprobar(Usuarios.Educador100, 3));
        }

        [Fact]
        public void Modificar_horario_solo_el_propio_o_administrador()
        {
            Assert.True(ReglasDeAcceso.PuedeModificarHorarioDe(Usuarios.Educador100, 100));
            Assert.False(ReglasDeAcceso.PuedeModificarHorarioDe(Usuarios.Educador100, 200));
            Assert.False(ReglasDeAcceso.PuedeModificarHorarioDe(Usuarios.CoordinadorGlobal, 200));
            Assert.True(ReglasDeAcceso.PuedeModificarHorarioDe(Usuarios.Administrador, 200));
        }

        [Fact]
        public void Solo_el_administrador_administra_accesos()
        {
            Assert.True(ReglasDeAcceso.PuedeAdministrarAccesos(Usuarios.Administrador));
            Assert.False(ReglasDeAcceso.PuedeAdministrarAccesos(Usuarios.Consultor));
            Assert.False(ReglasDeAcceso.PuedeAdministrarAccesos(Usuarios.CoordinadorGlobal));
        }

        private static UsuarioActual Usuario(string nombre) => nombre switch
        {
            "Administrador" => Usuarios.Administrador,
            "Consultor" => Usuarios.Consultor,
            "CoordinadorArea3" => Usuarios.CoordinadorArea3,
            "CoordinadorGlobal" => Usuarios.CoordinadorGlobal,
            _ => Usuarios.Educador100,
        };
    }
}
