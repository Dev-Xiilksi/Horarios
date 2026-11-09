using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class DiasService : IDiasService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public DiasService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<DiaDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await _context.Dias.Select(a => new DiaDTO
                {
                    Dia1 = a.Dia1,
                    Nombre = a.Nombre,
                    Habil = a.Habil
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
