using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.Constants;

namespace SistemaHorario.Application.Seguridad
{
    //Quien puede hacer que con los horarios. Reproduce lo que las pantallas ya mostraban u ocultaban,
    //pero aplicado en el servidor para que no dependa de la pantalla.
    public static class ReglasDeAcceso
    {
        public static bool VeTodo(UsuarioActual u) =>
            u.Rol == RolesSistema.Administrador
            || u.Rol == RolesSistema.Consultor
            || (u.Rol == RolesSistema.Coordinador && AreasEspeciales.ConVisionGlobal.Contains(u.IdArea));

        public static bool PuedeVerHorario(UsuarioActual u, int empleado, int area) =>
            u.IdEmpleado == empleado
            || VeTodo(u)
            || (u.Rol == RolesSistema.Coordinador && u.IdArea == area);

        public static bool PuedeVerArea(UsuarioActual u, int area) =>
            VeTodo(u) || (u.Rol == RolesSistema.Coordinador && u.IdArea == area);

        public static bool PuedeAprobar(UsuarioActual u, int areaDelHorario) =>
            u.Rol == RolesSistema.Administrador
            || u.Rol == RolesSistema.Consultor
            || (u.Rol == RolesSistema.Coordinador
                && (AreasEspeciales.ConVisionGlobal.Contains(u.IdArea) || u.IdArea == areaDelHorario));

        //crear o eliminar el horario de un empleado: el propio, o cualquiera si es administrador
        public static bool PuedeModificarHorarioDe(UsuarioActual u, int empleado) =>
            u.IdEmpleado == empleado || u.Rol == RolesSistema.Administrador;

        public static bool PuedeAdministrarAccesos(UsuarioActual u) =>
            u.Rol == RolesSistema.Administrador;

        public static List<HrHorario1DTO> FiltrarVisibles(UsuarioActual u, IEnumerable<HrHorario1DTO> horarios) =>
            horarios.Where(h => PuedeVerHorario(u, h.Empleado, h.Area)).ToList();
    }
}
