using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Tests.Fakes
{
    //usuarios de prueba: el area 3 es un area normal; la 14 tiene vision global
    public static class Usuarios
    {
        public static UsuarioActual Administrador => new(1, "Administrador", 3);
        public static UsuarioActual Consultor => new(2, "Consultor", 3);
        public static UsuarioActual CoordinadorArea3 => new(3, "Coordinador", 3);
        public static UsuarioActual CoordinadorGlobal => new(4, "Coordinador", 14);
        public static UsuarioActual Educador100 => new(100, "Educador", 3);
    }

    public class UsuarioFijo : IUsuarioActual
    {
        private readonly UsuarioActual? _usuario;
        public UsuarioFijo(UsuarioActual? usuario) => _usuario = usuario;
        public Task<UsuarioActual?> ObtenerAsync() => Task.FromResult(_usuario);
    }

    public class HorarioRepositoryFalso : IHorarioRepository
    {
        public Dictionary<int, HrHorario1> Horarios { get; } = new();
        public List<HrBitacoraConfirmacion> Confirmaciones { get; } = new();
        public bool HayAprobado { get; set; }
        public bool Eliminado { get; private set; }
        public HrHorario1? Agregado { get; private set; }

        public Task<HrHorario1?> Buscar(int idHorario1) =>
            Task.FromResult(Horarios.TryGetValue(idHorario1, out var h) ? h : null);

        public Task<bool> ExisteEnEstado(int empleado, string ciclo, int estado) => Task.FromResult(HayAprobado);

        public Task Agregar(HrHorario1 horario)
        {
            horario.IdHorario1 = 99;
            Agregado = horario;
            return Task.CompletedTask;
        }

        public Task<int> GuardarAprobacion(HrHorario1 horario, HrBitacoraConfirmacion confirmacion)
        {
            Confirmaciones.Add(confirmacion);
            return Task.FromResult(2);
        }

        public Task<int?> EliminarConDetalle(int empleado, string ciclo)
        {
            Eliminado = true;
            return Task.FromResult<int?>(50);
        }
    }

    public class HorarioConsultasFalsas : IHorarioConsultas
    {
        public List<HrHorario1DTO> Horarios { get; } = new();

        public Task<List<HrHorario1DTO>> ObtenerTodos(string ciclo) => Task.FromResult(Horarios.ToList());

        public Task<List<HrHorario1DTO>> ObtenerPorEstado(short area, string? ciclo, int estado) =>
            Task.FromResult(Horarios.Where(h => h.Area == area).ToList());

        public Task<HrHorario1DTO?> ObtenerPorId(int id) =>
            Task.FromResult(Horarios.FirstOrDefault(h => h.IdHorario1 == id));

        public Task<HrHorario1DTO?> ObtenerPorEmpleado(int empleado, string ciclo) =>
            Task.FromResult(Horarios.FirstOrDefault(h => h.Empleado == empleado));

        public Task<List<HrHorario2DTO>> ObtenerFranjas(int idHorario1) =>
            Task.FromResult(new List<HrHorario2DTO> { new() });
    }

    public class BitacoraFalsa : IHrBitacoraService
    {
        public List<(int? Usuario, int Horario, string? Accion)> Registros { get; } = new();

        public Task GuardarBitacora(int? usuario, int horario, string? accion, string? mensaje)
        {
            Registros.Add((usuario, horario, accion));
            return Task.CompletedTask;
        }
    }

    public class LogErrorFalso : ILogErrorService
    {
        public List<(string? Metodo, string? Usuario)> Errores { get; } = new();

        public Task GuardarError(Exception ex, string? metodo = null, string? usuario = null)
        {
            Errores.Add((metodo, usuario));
            return Task.CompletedTask;
        }
    }

    public class AccesoUsuarioRepositoryFalso : IAccesoUsuarioRepository
    {
        public AccesoUsuario Acceso { get; } = new() { IdAcceso = 1, IdEmpleado = 100, IdRol = 3, IdArea = 3 };
        public bool Modificado { get; private set; }

        public Task<List<AccesoUsuarioDTO>> ObtenerTodos() => Task.FromResult(new List<AccesoUsuarioDTO> { new() });
        public Task<AccesoUsuarioDTO?> ObtenerPorEmpleado(int idEmpleado) => Task.FromResult<AccesoUsuarioDTO?>(new());
        public Task<AccesoUsuarioDTO?> ObtenerPorId(int idAcceso) => Task.FromResult<AccesoUsuarioDTO?>(new());
        public Task<AccesoUsuario?> Buscar(int idAcceso) => Task.FromResult<AccesoUsuario?>(Acceso);
        public Task<AccesoUsuario?> BuscarPorEmpleado(int idEmpleado) => Task.FromResult<AccesoUsuario?>(Acceso);
        public Task Agregar(AccesoUsuario acceso) { Modificado = true; return Task.CompletedTask; }
        public Task<int> Actualizar(AccesoUsuario acceso) { Modificado = true; return Task.FromResult(1); }
        public Task Eliminar(AccesoUsuario acceso) { Modificado = true; return Task.CompletedTask; }
    }
}
