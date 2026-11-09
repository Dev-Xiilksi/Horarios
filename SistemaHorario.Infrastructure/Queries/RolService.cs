using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Infrastructure.Queries
{
    public class RolService : IRolService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        public RolService(IDbContextFactory<AcademiaContext> contextFactory) => _contextFactory = contextFactory; 
        public async Task<List<RolDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await _context.Rols
                    .Select(a => new RolDTO
                    {
                        IdRol = a.IdRol,
                        Nombre = a.Nombre,
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
