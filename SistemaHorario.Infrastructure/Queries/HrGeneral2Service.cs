using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class HrGeneral2Service : IHrGeneral2Service
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HrGeneral2Service(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HrGeneral2DTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await _context.HrGeneral2s.Select(a => new HrGeneral2DTO
                {
                    IdHrg2 = a.IdHrg2,
                    IdHrg1 = a.IdHrg1,
                    Horai = a.Horai,
                    Horaf = a.Horaf,
                    Tiempo = a.Tiempo,
                    FullValue = a.Horai+" a "+a.Horaf
                }).ToListAsync();
                return list;
            }
            catch (Exception)
            {

                throw;
            }
        }

    }
}
