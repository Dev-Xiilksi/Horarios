using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class ColegioNivelGradoSeccionService : IColegioNivelGradoSeccionService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public ColegioNivelGradoSeccionService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<ColegioNivelGradoSeccionDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            
            try
            {
                var list = await _context.CatalogoColegios
                    .Where(a => a.Colegio == "M" && !a.Nivel!.Contains("Y") && !a.Nivel.Contains("Z"))
                    .Select(a => new ColegioNivelGradoSeccionDTO
                    {
                        Colegio = a.Colegio,
                        Nivel = a.Nivel,
                        Grado = a.Grado,
                        Seccion = a.Seccion,
                        Nombre = a.Nivel + " - " + a.Nomgrado + " " + a.Seccion,
                    })
                    .OrderBy(a => a.Colegio)
                    .ThenBy(a => a.Nivel)
                    .ThenBy(a => a.Grado)
                    .ThenBy(a => a.Seccion)
                    .ToListAsync();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
