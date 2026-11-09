using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public UsuarioRepository(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<string?> ObtenerRolDeAcceso(int idEmpleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var acceso = await _context.AccesoUsuarios
                .Include(a => a.IdRolNavigation)
                .Where(a => a.IdEmpleado == idEmpleado)
                .FirstOrDefaultAsync();

            if (acceso == null)
                return null;

            return acceso.IdRolNavigation?.Nombre ?? string.Empty;
        }

        public async Task<bool> EsMaestro(int idEmpleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Maestros.AnyAsync(a => a.Empleado == idEmpleado);
        }

        public async Task<LoginDTO?> ObtenerDatosSesion(int idEmpleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await (
                from a in _context.Emplegens
                join b in _context.AccesoUsuarios on a.Empleado equals b.IdEmpleado
                join ar in _context.Areas on b.IdArea equals ar.Area1
                join ro in _context.Rols on b.IdRol equals ro.IdRol
                where a.Empleado == idEmpleado
                select new LoginDTO
                {
                    IdUser = Convert.ToString(a.Empleado),
                    UserName = a.Apellido1 + " " + a.Apellido2 + " " + a.Nombre1 + " " + a.Nombre2,
                    Rol = ro.Nombre.Trim(),
                    AreaName = ar.Nombre,
                    Area = b.IdArea
                }).FirstOrDefaultAsync();
        }
    }
}
