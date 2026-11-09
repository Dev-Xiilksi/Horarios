using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.Entities;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Repositories
{
    public class AccesoUsuarioRepository : IAccesoUsuarioRepository
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public AccesoUsuarioRepository(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<AccesoUsuarioDTO>> ObtenerTodos()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await ConsultaConNombres(_context).ToListAsync();
        }

        public async Task<AccesoUsuarioDTO?> ObtenerPorEmpleado(int idEmpleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await ConsultaConNombres(_context)
                .Where(a => a.IdEmpleado == idEmpleado)
                .FirstOrDefaultAsync();
        }

        public async Task<AccesoUsuarioDTO?> ObtenerPorId(int idAcceso)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.AccesoUsuarios
                .Where(a => a.IdAcceso == idAcceso)
                .Select(a => new AccesoUsuarioDTO
                {
                    IdAcceso = a.IdAcceso,
                    IdRol = a.IdRol,
                    IdEmpleado = a.IdEmpleado,
                    IdArea = a.IdArea
                })
                .FirstOrDefaultAsync();
        }

        public async Task<AccesoUsuario?> Buscar(int idAcceso)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.AccesoUsuarios.FindAsync(idAcceso);
        }

        public async Task<AccesoUsuario?> BuscarPorEmpleado(int idEmpleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.AccesoUsuarios.FirstOrDefaultAsync(a => a.IdEmpleado == idEmpleado);
        }

        public async Task Agregar(AccesoUsuario acceso)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            _context.AccesoUsuarios.Add(acceso);
            await _context.SaveChangesAsync();
        }

        public async Task<int> Actualizar(AccesoUsuario acceso)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            _context.AccesoUsuarios.Update(acceso);
            return await _context.SaveChangesAsync();
        }

        public async Task Eliminar(AccesoUsuario acceso)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            _context.AccesoUsuarios.Remove(acceso);
            await _context.SaveChangesAsync();
        }

        private static IQueryable<AccesoUsuarioDTO> ConsultaConNombres(AcademiaContext _context)
        {
            return from a in _context.AccesoUsuarios
                   join b in _context.Emplegens on a.IdEmpleado equals b.Empleado
                   join c in _context.Rols on a.IdRol equals c.IdRol
                   join d in _context.Areas on a.IdArea equals d.Area1
                   select new AccesoUsuarioDTO
                   {
                       IdAcceso = a.IdAcceso,
                       IdRol = a.IdRol,
                       IdEmpleado = a.IdEmpleado,
                       IdArea = a.IdArea,
                       FullName = b.Apellido1 + " " + b.Apellido2 + " " + b.Nombre1 + " " + b.Nombre2,
                       RolName = c.Nombre,
                       AreaName = d.Nombre
                   };
        }
    }
}
