using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;
using Microsoft.Extensions.Logging;

namespace SistemaHorario.Infrastructure.Queries
{
    public class EmployeeProfileService : IEmployeeProfileService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        private readonly ILogger<EmployeeProfileService> _logger;

        public EmployeeProfileService(IDbContextFactory<AcademiaContext> contextFactory, ILogger<EmployeeProfileService> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        public async Task<EmployeeProfileDTO> Get(string employee)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await (from a in _context.AccesoUsuarios
                              join b in _context.Emplegens on a.IdEmpleado equals b.Empleado
                              join c in _context.Rols on a.IdRol equals c.IdRol
                              join d in _context.Areas on a.IdArea equals d.Area1
                              where a.IdEmpleado == Convert.ToInt32(employee)
                              select new EmployeeProfileDTO
                              {
                                  IdAcceso = a.IdAcceso,
                                  IdRol = a.IdRol,
                                  IdEmpleado = a.IdEmpleado,
                                  IdArea = a.IdArea,
                                  FullName = b.Apellido1 + " " + b.Apellido2 + " " + b.Nombre1 + " " + b.Nombre2,
                                  RolName = c.Nombre,
                                  AreaName = d.Nombre
                              }).FirstOrDefaultAsync();
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener los datos.");
                throw;
            }
        }
    }
}
