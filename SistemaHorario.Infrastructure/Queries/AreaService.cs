using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;
using Microsoft.Extensions.Logging;
using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Infrastructure.Queries
{
    public class AreaService : IAreaService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        private readonly ILogger<AreaService> _logger;

        public AreaService(IDbContextFactory<AcademiaContext> contextFactory, ILogger<AreaService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<List<AreaDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var entity = await _context.Areas
                    //.Where(a => a.Area1 != 0 && a.Area1 != 11 && a.Area1 != 14 && a.Area1 != 15 && a.Area1 != 16)
                    .Select(a => new AreaDTO
                {
                    Areas = a.Area1,
                    Nombre = a.Nombre,
                    Código = a.Codigo.Trim()
                }).OrderBy(a=>a.Nombre).ToListAsync();

                return entity!;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las areas.");
                throw;
            }
        }

        public async Task<List<AreaDTO>> GetAreasWithTotalHrs(string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var areas = await (
                    from a in _context.Areas
                    join b in _context.HrHorario1s on a.Area1 equals b.Area
                    where b.Ciclo == ciclo
                    group b by new
                    {
                        a.Area1,
                        a.Nombre,
                        a.Codigo
                    } into g
                    select new AreaDTO
                    {
                        Areas = g.Key.Area1,
                        Nombre = g.Key.Nombre,
                        Código = g.Key.Codigo,
                        TotalPorArea = g.Count()
                    }
                ).ToListAsync();
                return areas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las areas y su total de horarios registrados.");
                throw;
            }
        }
    }
}
