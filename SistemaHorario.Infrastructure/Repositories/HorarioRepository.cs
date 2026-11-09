using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.Entities;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Repositories
{
    public class HorarioRepository : IHorarioRepository
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HorarioRepository(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<HrHorario1?> Buscar(int idHorario1)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            //sin seguimiento: cada operacion usa su propio contexto; GuardarAprobacion lo vuelve a adjuntar
            return await _context.HrHorario1s.AsNoTracking().FirstOrDefaultAsync(a => a.IdHorario1 == idHorario1);
        }

        public async Task<bool> ExisteEnEstado(int empleado, string ciclo, int estado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrHorario1s
                .AnyAsync(a => a.Ciclo == ciclo && a.Empleado == empleado && a.IdConfirmacion == estado);
        }

        public async Task Agregar(HrHorario1 horario)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            _context.HrHorario1s.Add(horario);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GuardarAprobacion(HrHorario1 horario, HrBitacoraConfirmacion confirmacion)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            //se adjunta el horario y solo se marca el estado de confirmacion; el resto de columnas no se toca
            _context.HrHorario1s.Attach(horario);
            _context.Entry(horario).Property(h => h.IdConfirmacion).IsModified = true;
            _context.HrBitacoraConfirmacion.Add(confirmacion);
            return await _context.SaveChangesAsync();
        }

        public async Task<int?> EliminarConDetalle(int empleado, string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var entity = await _context.HrHorario1s
                .Include(h => h.HrHorario2s)
                .Include(h => h.HrHorarioExtras)
                .Include(h => h.HrGrados)
                .FirstOrDefaultAsync(h => h.Empleado == empleado && h.Ciclo == ciclo);

            if (entity == null)
                return null;

            // eliminar hijos primero
            var bitacorconfirm = await _context.HrBitacoraConfirmacion
                .Where(a => a.id_horario1 == entity.IdHorario1)
                .ToListAsync();
            _context.HrBitacoraConfirmacion.RemoveRange(bitacorconfirm);

            if (entity.HrHorario2s.Any())
                _context.HrHorario2s.RemoveRange(entity.HrHorario2s);

            if (entity.HrGrados.Any())
                _context.HrGrados.RemoveRange(entity.HrGrados);

            // luego eliminar el padre
            _context.HrHorario1s.Remove(entity);
            await _context.SaveChangesAsync();

            return entity.IdHorario1;
        }
    }
}
