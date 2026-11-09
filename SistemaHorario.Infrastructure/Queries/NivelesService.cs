using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class NivelesService : INivelesService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public NivelesService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<NivelDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var entity = await _context.Niveles
                .Where(a => a.Colegio == "M" && a.Nivel != "Y" && a.Nivel != "Z")
                .Select(a => new NivelDTO
            {
                Colegio = a.Colegio,
                Nivel = a.Nivel,
                Nombre = a.Nombre,
                Orden = a.Orden,
                Promo = a.Promo
            }).ToListAsync();

            //entity.Add(new NivelDTO
            //{
            //    Colegio = "X",
            //    Nivel = "V",
            //    Nombre = "VARIADO",
            //    Orden = 0,
            //    Promo = "X"
            //});


            return entity;
        }
    }
}
