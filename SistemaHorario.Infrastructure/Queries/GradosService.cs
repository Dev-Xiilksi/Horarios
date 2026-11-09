using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class GradosService : IGradosService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public GradosService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<GradoDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var entity = await _context.Grados
                    .Where(a => a.Colegio == "M" && a.Nivel != "Y" && a.Nivel != "Z")
                    .Select(a => new GradoDTO
                    {
                        Colegio = a.Colegio,
                        Nivel = a.Nivel,
                        Grado1 = a.Grado1,
                        Nombre = a.Nombre,
                        Origen = a.Origen,
                        Real = a.Real
                    }).ToListAsync();

                return entity;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<GradoDTO>> GetAllFilter()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var entity = await _context.Grados
                    .Where(a => a.Colegio == "M" && a.Nivel != "Y" && a.Nivel != "Z")
                    .Select(a => new GradoDTO
                    {
                        Colegio = a.Colegio,
                        Nivel = a.Nivel,
                        Grado1 = a.Grado1,
                        Nombre = a.Nombre,
                        Origen = a.Origen,
                        Real = a.Real
                    }).ToListAsync();
                return entity;
            }
            catch (Exception)
            {
                throw;
            }
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
