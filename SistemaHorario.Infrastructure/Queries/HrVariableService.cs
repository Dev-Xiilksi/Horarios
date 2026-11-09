using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class HrVariableService : IHrVariableService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HrVariableService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HrVariableDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await _context.HrVariable
                    .Select(a => new HrVariableDTO
                    {
                        IdHrVariable = a.IdHrVariable,
                        HorarioI = a.HorarioI,
                        HorarioF = a.HorarioF
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
