using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Queries
{
    //lecturas de horarios; sin seguimiento de cambios porque solo se proyectan DTOs
    public class HorarioConsultas : IHorarioConsultas
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;

        public HorarioConsultas(IDbContextFactory<AcademiaContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<List<HrHorario1DTO>> ObtenerTodos(string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var query = from h in _context.HrHorario1s
                        join g in _context.HrGenerals on h.IdHrg1 equals g.IdHrg1 into general
                        from g in general.DefaultIfEmpty()
                        join a in _context.Areas on h.Area equals a.Area1
                        join b in _context.Emplegens on h.Empleado equals b.Empleado
                        where h.Empresa == 1 && h.Ciclo == ciclo
                        orderby h.IdHorario1
                        select new HrHorario1DTO
                        {
                            IdHorario1 = h.IdHorario1,
                            IdHrg1 = h.IdHrg1,
                            Empresa = h.Empresa,
                            Empleado = h.Empleado,
                            Ciclo = h.Ciclo,
                            NombreGeneral = g.Nombre,
                            NombreEmpleado = b.Apellido1 + " " + b.Apellido2 + " " + b.Nombre1 + " " + b.Nombre2,
                            Area = Convert.ToInt16(h.Area),
                            Colegio1 = h.Colegio,
                            Nivel = h.Nivel,
                            Grado = h.Grado,
                            NombreArea = a.Nombre,
                            Colegio = h.Colegio + h.Nivel + h.Grado + h.Seccion,
                            IdConfirmacion = h.IdConfirmacion,
                            TotalPorArea = _context.HrHorario1s.Count(x =>
                                            x.Area == h.Area &&
                                            x.Empresa == h.Empresa &&
                                            x.Ciclo == h.Ciclo),
                            PoseeHorario2 = h.HrHorario2s.Count(),
                            Fecha_Registro = h.Fecha_Registro,
                            Hora_Registro = h.Hora_Registro,
                            TipoFranja = h.TipoFranja
                        };

            return await query.ToListAsync();
        }

        public async Task<List<HrHorario1DTO>> ObtenerPorEstado(short area, string? ciclo, int estado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var query = from h in _context.HrHorario1s
                        join g in _context.HrGenerals on h.IdHrg1 equals g.IdHrg1
                        join a in _context.Areas on h.Area equals a.Area1
                        where h.Empresa == 1 && h.Area == area && h.Ciclo == ciclo && h.IdConfirmacion == estado
                        orderby h.IdHorario1
                        select new HrHorario1DTO
                        {
                            IdHorario1 = h.IdHorario1,
                            NombreGeneral = g.Nombre,
                            NombreArea = a.Nombre,
                            Colegio = h.Colegio + h.Nivel + h.Grado + h.Seccion,
                        };
            return await query.ToListAsync();
        }

        public async Task<HrHorario1DTO?> ObtenerPorId(int id)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrHorario1s
                .Where(a => a.IdHorario1 == id)
                .Select(a => new HrHorario1DTO
                {
                    IdHorario1 = a.IdHorario1,
                    IdHrg1 = a.IdHrg1,
                    Empresa = a.Empresa,
                    Empleado = a.Empleado,
                    Ciclo = a.Ciclo,
                    Area = Convert.ToInt16(a.Area),
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    TipoFranja = a.TipoFranja
                })
                .FirstOrDefaultAsync();
        }

        public async Task<HrHorario1DTO?> ObtenerPorEmpleado(int empleado, string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrHorario1s
                .Where(a => a.Empleado == empleado && a.Ciclo == ciclo)
                .Select(a => new HrHorario1DTO
                {
                    IdHorario1 = a.IdHorario1,
                    IdHrg1 = a.IdHrg1,
                    Empresa = a.Empresa,
                    Empleado = a.Empleado,
                    Ciclo = a.Ciclo,
                    Area = Convert.ToInt16(a.Area),
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    Fecha_Registro = a.Fecha_Registro,
                    IdConfirmacion = a.IdConfirmacion,
                    TipoFranja = a.TipoFranja
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<HrHorario2DTO>> ObtenerFranjas(int idHorario1)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrHorario2s
                .Where(a => a.IdHorario1 == idHorario1)
                .Select(a => new HrHorario2DTO
                {
                    IdHorario1 = a.IdHorario1,
                    Dia = a.Dia,
                    IdHrg2 = a.IdHrg2,
                    IdMps = a.IdMps,
                    IdHra = a.IdHra,
                    Horai = a.Horai,
                    Horaf = a.Horaf,
                    Tiempo = a.Tiempo,
                    NumeroFila = a.NumeroFila,
                    EsLectivo = a.IdMps != null ? "Lectivo" : "No Lectivo"
                })
                .ToListAsync();
        }
    }
}
