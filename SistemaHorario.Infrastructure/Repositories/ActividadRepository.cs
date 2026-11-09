using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Repositories
{
    public class ActividadRepository : IActividadRepository
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public ActividadRepository(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<MateriaImpartidaDto>> ObtenerMateriasImpartidas(int empleado, string ciclo, short empresa)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var query =
                from a in _context.Maestrosporseccions
                join b in _context.Maestros on a.Codigo equals b.Codigo
                join c in _context.Materias on new { x1 = a.Codmateria, x2 = a.Colegio } equals new { x1 = c.Codigo, x2 = c.Colegio }
                join d in _context.Emplegens on new { x1 = b.Empleado, x2 = empresa } equals new { x1 = d.Empleado, x2 = d.Empresa }
                where a.Ciclo == ciclo
                      && b.Empleado == empleado
                select new MateriaImpartidaDto
                {
                    IdMps = a.IdMps,
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    Seccion = a.Seccion,
                    NombreMateria = c.Nombre
                };

            return await query.ToListAsync();
        }

        public async Task<List<ActividadDTO>> ObtenerActividades()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrActividades
                .Select(a => new ActividadDTO
                {
                    Id = Convert.ToInt16(a.IdHra),
                    Name = a.Nombre,
                    Description = a.Descripcion,
                    Type = "Actividad",
                    TipoOpcion = a.IdTipoOpcion
                })
                .ToListAsync();
        }

        public async Task<List<ActividadDTO>> ObtenerSeccionesDelColegio()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.CatalogoColegios
                .Where(a => a.Colegio == "M" && a.Nivel != "Z" && a.Nivel != "Y")
                .Select(a => new ActividadDTO
                {
                    Id = a.Id,
                    Name = a.Colegio + a.Nivel + a.Grado + a.Seccion,
                    Description = "No Descripticion",
                    Type = "Grado"
                })
                .ToListAsync();
        }
    }
}
