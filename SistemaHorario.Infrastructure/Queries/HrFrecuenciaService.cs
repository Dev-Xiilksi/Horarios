using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SistemaHorario.Infrastructure.Queries
{
    public class HrFrecuenciaService : IHrFrecuenciaService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HrFrecuenciaService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HrFrecuenciaDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = _context.HrFrecuencia.Select(a => new HrFrecuenciaDTO
                {
                    IdHrf = a.IdHrf,
                    Descripcion = a.Descripcion,
                    Activo = a.Activo
                }).ToListAsync();

                return await list;
            }
            catch (Exception)
            {
                throw;
            }
        }

    }
}
