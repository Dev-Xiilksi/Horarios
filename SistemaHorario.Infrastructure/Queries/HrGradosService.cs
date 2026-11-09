using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class HrGradosService : IHrGradosService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HrGradosService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public async Task<List<HrGradosDto>> ObtenerGrados(int idhorario1)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var entity = await _context.HrGrados
                .Where(a => a.IdHorario1 == idhorario1)
                .Select(a => new HrGradosDto
                {
                    IdHrGrados = a.IdHrGrados,
                    IdHorario1 = a.IdHorario1,
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    Seccion = a.Seccion,
                    NivelGrado = a.NivelGrado
                }).ToListAsync();

            return entity;
        }
    }
}
