using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace SistemaHorario.Infrastructure.Queries
{
    public class EmplegenService : IEmplegenService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public EmplegenService(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        //obtiene los empleados activos
        public async Task<List<EmplegenDTO>> GetAll()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var entity = await _context.Contratos1s
                    .Where(a => a.Empresa == 1 && a.Estado == 0)
                    .Include(a => a.Emplegen).Select(a => new EmplegenDTO
                    {
                        Empresa = a.Empresa,
                        Empleado = a.Empleado,
                        FullName = a.Emplegen.Apellido1 +" "+a.Emplegen.Apellido2+" "+a.Emplegen.Nombre1 + " " + a.Emplegen.Nombre2 + " " + a.Emplegen.Nombre3
                    })
                    .Distinct()
                    .ToListAsync();
                return entity!;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
