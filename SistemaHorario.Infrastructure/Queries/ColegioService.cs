using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class ColegioService : IColegioService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public ColegioService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ColegioDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var entity = await _context.Colegios.Select(a => new ColegioDTO
                {
                    Colegio1 = a.Colegio1,
                    Nombre = a.Nombre,
                    Empresa = a.Empresa,
                    Notaoficial = a.Notaoficial,
                    Notacolegio = a.Notacolegio
                }).ToListAsync();
                return entity;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
