using Dapper;
using Microsoft.EntityFrameworkCore;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.Constants;
using SistemaHorario.Infrastructure.Persistence;

namespace SistemaHorario.Infrastructure.Repositories
{
    public class ReporteRepository : IReporteRepository
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        private readonly DapperContext _dapper;

        public ReporteRepository(IDbContextFactory<AcademiaContext> contextFactory, DapperContext dapper)
        {
            _contextFactory = contextFactory;
            _dapper = dapper;
        }

        public async Task<List<HorarioAuxDTO>> ObtenerDetalleParaExportar(string? ciclo, int area)
        {
            var query = SqlDetalleParaExportar;
            if (area != 0)
                query += " and a.area=@area";

            using var connection = _dapper.Crearconexion();
            var list = await connection.QueryAsync<HorarioAuxDTO>(query, new { ciclo, area });
            return list.ToList();
        }

        public async Task<List<HorarioAuxDTO>> ObtenerHorariosResumen(string ciclo)
        {
            using var connection = _dapper.Crearconexion();
            var list = await connection.QueryAsync<HorarioAuxDTO>(SqlHorariosResumen, new { ciclo });
            return list.ToList();
        }

        public async Task<List<HorarioConsolidadoDto>> ObtenerConsolidados(string ciclo, string? area)
        {
            var query = SqlConsolidados;
            if (area != null)
                query += " and a.area = @area";

            using var connection = _dapper.Crearconexion();
            var list = await connection.QueryAsync<HorarioConsolidadoDto>(query, new { ciclo, area });
            return list.ToList();
        }

        public async Task<ColaboradorDTO?> ObtenerColaborador(int empleado, string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            var query =
                from a in _context.HrHorario1s
                join b in _context.HrGenerals on a.IdHrg1 equals b.IdHrg1 into gj1
                from b in gj1.DefaultIfEmpty()
                join c in _context.Emplegens on a.Empleado equals c.Empleado into gj2
                from c in gj2.DefaultIfEmpty()
                join d in _context.Areas on a.Area equals d.Area1 into gj3
                from d in gj3.DefaultIfEmpty()
                join e in _context.Maestros on a.Empleado equals e.Empleado into gj4
                from e in gj4.DefaultIfEmpty()
                join gr in _context.HrGrados on a.IdHorario1 equals gr.IdHorario1 into gj5
                from gr in gj5.DefaultIfEmpty()
                where a.Empleado == empleado && a.Ciclo == ciclo
                select new ColaboradorDTO
                {
                    IdHorario = a.IdHorario1,
                    Horario = b != null ? b.Nombre : "",
                    NumEmpleado = a.Empleado,
                    Educador = c != null ? ($"{c.Apellido1} {c.Apellido2} {c.Nombre1} {c.Nombre2}") : "",
                    Area = d != null ? d.Nombre : "",
                    Codigo = e != null ? e.Codigo : 0,
                    TipoFranja = TipoFranja.Descripcion(a.TipoFranja)
                };

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<HrGradosDto>> ObtenerGrados(int idHorario1)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrGrados
                .Where(a => a.IdHorario1 == idHorario1)
                .Select(a => new HrGradosDto
                {
                    IdHorario1 = a.IdHorario1,
                    Colegio = a.Colegio,
                    Nivel = a.Nivel,
                    Grado = a.Grado,
                    Seccion = a.Seccion
                })
                .ToListAsync();
        }

        public async Task<Dictionary<int, string>> ObtenerMateriasDelMaestro(int codigoMaestro, string ciclo)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.Maestrosporseccions
                .Where(mps => mps.Codigo == codigoMaestro && mps.Ciclo == ciclo)
                .Join(_context.Materias,
                      mps => new { CodMateria = mps.Codmateria, Colegio = mps.Colegio },
                      mat => new { CodMateria = mat.Codigo, Colegio = mat.Colegio },
                      (mps, mat) => new { mps.IdMps, NombreMateria = mat.Nombre + " (" + mps.Colegio + mps.Nivel + mps.Grado + mps.Seccion + ")" })
                .ToDictionaryAsync(m => m.IdMps, m => m.NombreMateria);
        }

        public async Task<Dictionary<short, string>> ObtenerActividades()
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrActividades
                .ToDictionaryAsync(a => a.IdHra, a => a.Nombre);
        }

        public async Task<List<FranjaHorariaDto>> ObtenerFranjas(int idHorario1)
        {
            await using var _context = await _contextFactory.CreateDbContextAsync();
            return await _context.HrHorario2s
                .Where(x => x.IdHorario1 == idHorario1)
                .Select(a => new FranjaHorariaDto
                {
                    Horai = a.Horai,
                    Horaf = a.Horaf,
                    Tiempo = a.Tiempo,
                    Dia = a.DiaNavigation!.Nombre!,
                    IdMps = a.IdMps,
                    IdHra = a.IdHra
                })
                .ToListAsync();
        }

        #region Consultas SQL
        private const string SqlDetalleParaExportar = @"SELECT a.empleado as Codigo,em.apellido1+' '+em.apellido2+' '+em.nombre1+' '+em.nombre2 as Educador, a.ciclo,ar.nombre as Area,a.colegio,a.nivel,a.grado,a.seccion,a.correcion_trabajos,a.estudiantes,a.observacion,
                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 1), '') AS [ADM],
                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 2), '') AS [ECC],
                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 3), '') AS [EUC],
					            ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 4), '') AS [CAMM],
					            ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 6), '') AS [EF], 
					            ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 7), '') AS [SSE],
					            ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 8), '') AS [RPMF], 
					            ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 9), '') AS [TECHO],
                              
                                (SELECT CAST(total / 3600 AS VARCHAR) + ':' + 
                                        FORMAT(total % 3600 / 60, '00') + ':' + 
                                        FORMAT(total % 60, '00')
                                 FROM (SELECT SUM(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                  CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                  ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0)) AS total   
                                       FROM hr_horario2 a3
                                       WHERE id_horario1 = a.id_horario1 AND id_mps > 0) LL) AS [IA],
                            
                            piv.[AAA],piv.[ALM],piv.[CUIDA],piv.[FMAGIS],piv.[GAA],piv.[GAO],piv.[ORA],piv.[RAREA],piv.[RCOMUNI],piv.[RORIENT],piv.[RPERSONAL],piv.[RTP],piv.[ANP],
                            
                            (SELECT CAST(SUM(total_segundos) / 3600 AS VARCHAR) + ':' + 
                                    FORMAT(SUM(total_segundos) % 3600 / 60, '00') + ':' + 
                                    FORMAT(SUM(total_segundos) % 60, '00')
                             FROM (
                                SELECT SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                 CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                 ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                FROM hr_horario2 b2
                                WHERE b2.id_horario1 = a.id_horario1
                             ) totales) AS [TOTAL]
                            
                            FROM hr_horario1 a
                            inner join emplegen em on a.empleado=em.empleado
                            inner join areas ar on a.area=ar.area
                            CROSS APPLY (
                                SELECT *
                                FROM (
                                    SELECT codigo, 
                                           CAST(total_segundos / 3600 AS VARCHAR) + ':' + 
                                           FORMAT(total_segundos % 3600 / 60, '00') + ':' + 
                                           FORMAT(total_segundos % 60, '00') as tiempo_formateado
                                    FROM (
                                        SELECT b1.codigo, 
                                               SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                   CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                   ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                        FROM hr_actividades b1
                                        LEFT JOIN hr_horario2 b2 ON b1.id_hra = b2.id_hra AND b2.id_horario1 = a.id_horario1
                                        GROUP BY b1.codigo 
                                    ) LL
                                ) datos
                                PIVOT (
                                    MAX(tiempo_formateado)
                                    FOR codigo IN ([AAA], [ALM], [CUIDA], [FMAGIS], [GAA], [GAO], [ORA], 
                                                  [RAREA], [RCOMUNI], [RORIENT], [RPERSONAL], [RTP], [ANP])
                                ) pvt
                            ) piv
                            WHERE a.ciclo = @ciclo";

        private const string SqlHorariosResumen = @"SELECT a.empleado as Codigo,em.apellido1+' '+em.apellido2+' '+em.nombre1+' '+em.nombre2 as Educador, a.ciclo,ar.nombre as Area,a.colegio,a.nivel,a.grado,a.seccion,a.correcion_trabajos as CTFA,a.estudiantes,a.observacion,

                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 1), '') AS [ADM],
                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 2), '') AS [ECC],
                                ISNULL((SELECT MAX(a2.descripcion)  
                                        FROM hr_horario_extra a1
                                        INNER JOIN hr_frecuencia a2 ON a1.id_hrf = a2.id_hrf
                                        WHERE a1.id_horario1 = a.id_horario1 AND a1.id_hex = 3), '') AS [EUC],
                              
                                (SELECT CAST(total / 3600 AS VARCHAR) + ':' + 
                                        FORMAT(total % 3600 / 60, '00') + ':' + 
                                        FORMAT(total % 60, '00')
                                 FROM (SELECT SUM(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                  CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                  ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0)) AS total   
                                       FROM hr_horario2 a3
                                       WHERE id_horario1 = a.id_horario1 AND id_mps > 0) LL) AS [IA],
                            piv.[AAA],piv.[ALM],piv.[CUIDA],piv.[FMAGIS],piv.[GAA],piv.[GAO],piv.[ORA],piv.[RAREA],piv.[RCOMUNI],piv.[RORIENT],piv.[RPERSONAL],piv.[RTP],piv.[ANP]
                            
                            FROM hr_horario1 a
                            inner join areas ar on a.area=ar.area
                            inner join emplegen em on a.empleado=em.empleado
                            CROSS APPLY (
                                SELECT *
                                FROM (
                                    SELECT codigo, 
                                           CAST(total / 3600 AS VARCHAR) + ':' + 
                                           FORMAT(total % 3600 / 60, '00') + ':' + 
                                           FORMAT(total % 60, '00') as tiempo_formateado
                                    FROM (
                                        SELECT b1.codigo, 
                                               SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                   CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                   ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total   
                                        FROM hr_actividades b1
                                        LEFT JOIN hr_horario2 b2 ON b1.id_hra = b2.id_hra AND b2.id_horario1 = a.id_horario1
                                        GROUP BY b1.codigo 
                                    ) LL
                                ) datos
                                PIVOT (
                                    MAX(tiempo_formateado)
                                    FOR codigo IN ([AAA], [ALM], [CUIDA], [FMAGIS], [GAA], [GAO], [ORA], 
                                                  [RAREA], [RCOMUNI], [RORIENT], [RPERSONAL], [RTP], [ANP])
                                ) pvt
                            ) piv
                            WHERE a.ciclo = @ciclo";

        private const string SqlConsolidados = @"SELECT a.empleado as Codigo,em.apellido1+' '+em.apellido2+' '+em.nombre1+' '+em.nombre2 as Educador,a.ciclo,ar.nombre as area,
                                    coalesce((SELECT CAST(total / 3600 AS VARCHAR) + ':' +  FORMAT(total % 3600 / 60, '00') + ':' +  FORMAT(total % 60, '00')
                                     FROM (SELECT SUM(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 + ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0)) AS total   
                                           FROM hr_horario2 a3
                                           WHERE id_horario1 = a.id_horario1 AND id_mps > 0) LL),'00:00:00') AS [IA],
                                
                                piv.[AAA],piv.[ALM],piv.[CUIDA],piv.[FMAGIS],piv.[GAA],piv.[GAO],piv.[ORA],piv.[RAREA],piv.[RCOMUNI],piv.[RORIENT],piv.[RPERSONAL],piv.[RTP],piv.[ANP],
                                
                                (SELECT CAST(SUM(total_segundos) / 3600 AS VARCHAR) + ':' + 
                                        FORMAT(SUM(total_segundos) % 3600 / 60, '00') + ':' + 
                                        FORMAT(SUM(total_segundos) % 60, '00')
                                 FROM (
                                    SELECT SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                     CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                     ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                    FROM hr_horario2 b2
                                    WHERE b2.id_horario1 = a.id_horario1
                                 ) totales) AS [TOTAL]
                                
                                FROM hr_horario1 a
                                inner join emplegen em on a.empleado=em.empleado
                                inner join areas ar on a.area=ar.area
                                CROSS APPLY (
                                    SELECT *
                                    FROM (
                                        SELECT codigo, 
                                               CAST(total_segundos / 3600 AS VARCHAR) + ':' + 
                                               FORMAT(total_segundos % 3600 / 60, '00') + ':' + 
                                               FORMAT(total_segundos % 60, '00') as tiempo_formateado
                                        FROM (
                                            SELECT b1.codigo, 
                                                   SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                       CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                       ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                            FROM hr_actividades b1
                                            LEFT JOIN hr_horario2 b2 ON b1.id_hra = b2.id_hra AND b2.id_horario1 = a.id_horario1
                                            GROUP BY b1.codigo 
                                        ) LL
                                    ) datos
                                    PIVOT (
                                        MAX(tiempo_formateado)
                                        FOR codigo IN ([AAA], [ALM], [CUIDA], [FMAGIS], [GAA], [GAO], [ORA], 
                                                      [RAREA], [RCOMUNI], [RORIENT], [RPERSONAL], [RTP],[ANP])
                                    ) pvt
                                ) piv
                                WHERE a.ciclo = @ciclo";
        #endregion
    }
}
