using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.ValueObjects;
using SistemaHorario.Infrastructure.Persistence;
using SistemaHorario.Application.DTOs.Auxiliares;
using Microsoft.EntityFrameworkCore;

namespace SistemaHorario.Infrastructure.Queries
{
    public class ListadoHorarioColaboradorService : IListadoHorarioColaboradorService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        private readonly DapperContext _dapperContext;

        public ListadoHorarioColaboradorService(IDbContextFactory<AcademiaContext> contextFactory, DapperContext dapperContext)
        {
            _contextFactory = contextFactory;
            _dapperContext = dapperContext;
        }

        public async Task<List<ColaboradorDTO>> Colaborador(int idempleado)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            string ciclo = CicloEscolar.Actual();
            try
            {
                var list = await (
                    from a in _context.HrHorario1s
                    join b in _context.Emplegens on a.Empleado equals b.Empleado
                    join c in _context.Areas on a.Area equals c.Area1
                    join d in _context.HrTipoFranja on a.TipoFranja equals d.IdTipoFranja
                    where a.Ciclo == ciclo && b.Empleado == idempleado
                    select new ColaboradorDTO
                    {
                        Educador = b.Apellido1 + ' ' + b.Apellido2 + ' ' + b.Nombre1 + ' ' + b.Nombre2,
                        Area = c.Nombre,
                        Colegio = a.Colegio,
                        Nivel = a.Nivel,
                        Grado = a.Grado,
                        Seccion = a.Seccion,
                        CorreccionTrabajo = a.CorrecionTrabajos,
                        NoEstudiantes = a.Estudiantes,
                        Observacion = a.Observacion,
                        Codigo = b.Empleado,
                        Idconfirmacion = a.IdConfirmacion,
                        FechaRegistro = a.Fecha_Registro.ToShortDateString(), 
                        TipoFranja = d.Nombre
                    }
                    ).ToListAsync();

                return  list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ActividadAuxDTO>> ListaActividades(int idHorario)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await _context.HrHorario2s
                    .Include(x => x.DiaNavigation)
                    .Where(x => x.IdHorario1==idHorario)
                    .Select(a => new ActividadAuxDTO
                    {
                        Franja = a.IdHorario2,
                        Horai = a.Horai,
                        Horaf = a.Horaf,
                        Tiempo = a.Tiempo,
                        Dia = a.DiaNavigation!.Nombre,
                        idMps = a.IdMps,
                        idHra = a.IdHra
                    }).ToListAsync();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ColaboradorDTO>> ListaColaboradores()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            string ciclo = CicloEscolar.Actual();
            try
            {
                var list = await (
                    from a in _context.HrHorario1s
                    join b in _context.HrGenerals on a.IdHrg1 equals b.IdHrg1
                    join c in _context.Emplegens on a.Empleado equals c.Empleado
                    join d in _context.Areas on a.Area equals d.Area1
                    where a.Ciclo == ciclo
                    select new ColaboradorDTO
                    {
                        Horario = b.Nombre,
                        Educador = c.Apellido1+' '+c.Apellido2+' '+c.Nombre1+' '+c.Nombre2,
                        Area = d.Nombre,
                        Colegio = a.Colegio,
                        Nivel = a.Nivel,
                        Grado = a.Grado,
                        Seccion = a.Seccion,
                        CorreccionTrabajo = a.CorrecionTrabajos,
                        NoEstudiantes = a.Estudiantes,
                        Observacion = a.Observacion,
                        Codigo = c.Empleado
                    }
                    ).ToListAsync();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<ColaboradorDTO>> ListaColaboradoresVariables()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            string ciclo = CicloEscolar.Actual();
            try
            {
                var list = await (from a in _context.HrHorario1s
                                 join c in _context.Emplegens on a.Empleado equals c.Empleado
                                 join d in _context.Areas on a.Area equals d.Area1
                                 join e in _context.Maestros on a.Empleado equals e.Empleado
                                 join f in _context.HrHorario2s on a.IdHorario1 equals f.IdHorario1
                                 where a.Ciclo == ciclo
                                 select new ColaboradorDTO
                                 {
                                     Educador = c.Apellido1 + ' ' + c.Apellido2 + ' ' + c.Nombre1 + ' ' + c.Nombre2,
                                     Area = d.Nombre,
                                     Colegio = a.Colegio,
                                     Nivel = a.Nivel,
                                     Grado = a.Grado,
                                     Seccion = a.Seccion,
                                     CorreccionTrabajo = a.CorrecionTrabajos,
                                     NoEstudiantes = a.Estudiantes,
                                     Observacion = a.Observacion,
                                     Codigo = c.Empleado
                                 })
                                .Distinct()
                                .ToListAsync();

                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<EventoDTO>> ListaEventos(int idHorario)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            try
            {
                var list = await (
                    from a in _context.HrHorarioExtras
                    join b in _context.HrEventos on a.IdHex equals b.IdHex
                    join c in _context.HrFrecuencia on a.IdHrf equals c.IdHrf
                    where a.IdHorario1 == idHorario
                    select new EventoDTO
                    {
                        NombreEvento = b.Nombre,
                        Descripcion = c.Descripcion
                    }
                    ).ToListAsync();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
