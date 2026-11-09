using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SistemaHorario.Infrastructure.Queries
{
    public class HrGeneralService : IHrGeneralService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HrGeneralService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HrGeneralDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = _context.HrGenerals.Include(a => a.HrGeneral2s).Select(a => new HrGeneralDTO
                {
                    IdHrg1 = a.IdHrg1,
                    Nombre = a.Nombre,
                    Ciclo = a.Ciclo,
                    HrGeneral2 = a.HrGeneral2s.Select(a => new HrGeneral2DTO
                    {
                        IdHrg2 = a.IdHrg2,
                        IdHrg1 = a.IdHrg1,
                        Horai = a.Horai,
                        Horaf = a.Horaf,
                        Tiempo = a.Tiempo,
                    }).ToList(),
                    FullValue = a.Nombre + " " +
                    string.Join(" | ", a.HrGeneral2s.Select(h2 =>
                        $"{h2.Horai}-{h2.Horaf} ({h2.Tiempo})")),
                    //Horai = a.HrGeneral2s.Select(x => x.Horai).ToString(),
                    //Horaf = a.HrGeneral2s.Select(x => x.Horaf).ToString(),
                    //TiempoTotal = a.HrGeneral2s.Select(x => x.Tiempo).ToString(),
                }).ToListAsync();

                return await list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<HrGeneralDTO>> GeneralList()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = _context.HrGenerals
                    .Select(a => new HrGeneralDTO
                    {
                        IdHrg1 = a.IdHrg1,
                        Nombre = a.Nombre,
                        Ciclo = a.Ciclo,
                        HrGeneral2 = a.HrGeneral2s.Select(a => new HrGeneral2DTO
                        {
                            IdHrg2 = a.IdHrg2,
                            IdHrg1 = a.IdHrg1,
                            Horai = a.Horai,
                            Horaf = a.Horaf,
                            Tiempo = a.Tiempo
                        }).ToList(),
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
