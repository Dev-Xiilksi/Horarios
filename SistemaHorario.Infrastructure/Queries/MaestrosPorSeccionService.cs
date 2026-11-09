using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.ValueObjects;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class MaestrosPorSeccionService : IMaestrosPorSeccionService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public MaestrosPorSeccionService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<MaestrosporseccionDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            string year = CicloEscolar.Actual();
            try
            {
                var list = await _context.Maestrosporseccions.Select(a => new MaestrosporseccionDTO
                {
                    Codigo = a.Codigo,
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    Seccion = a.Seccion,
                    Codmateria = a.Codmateria,
                    IdMps = a.IdMps,
                    Ciclo = a.Ciclo
                }).Where(a => a.Ciclo == year).ToListAsync();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
