using Microsoft.EntityFrameworkCore;
﻿using SistemaHorario.Application.Interfaces;
using SistemaHorario.Infrastructure.Persistence;
using Dapper;
using SistemaHorario.Application.DTOs.Auxiliares;
using Microsoft.Extensions.Logging;

namespace SistemaHorario.Infrastructure.Queries
{
    public class DataToChartsService : IDataToChartsService
    {
        private readonly IDbContextFactory<AcademiaContext> _contextFactory;
        private readonly ILogger<DataToChartsService> _logger;
        private readonly DapperContext _dapper;

        public DataToChartsService(IDbContextFactory<AcademiaContext> contextFactory, ILogger<DataToChartsService> logger, DapperContext dapper)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _dapper = dapper;
        }

        public async Task<HorarioAuxDTO> GetHour(string employee)
        {
            try
            {
                using var connection = _dapper.Crearconexion();
                var query = @"SELECT a.empleado as Codigo,em.apellido1+' '+em.apellido2+' '+em.nombre1+' '+em.nombre2 as Educador, a.ciclo,ar.nombre as Area,a.colegio,a.nivel,a.grado,a.seccion,a.correcion_trabajos,a.estudiantes,a.observacion,
                                
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
                            
                            piv.[AAA],piv.[ALM],piv.[CUIDA],piv.[FMAGIS],piv.[GAA],piv.[GAO],piv.[ORA],piv.[RAREA],piv.[RCOMUNI],piv.[RORIENT],piv.[RPERSONAL],piv.[RTP],
                            
                            (SELECT CAST(SUM(total_segundos) / 3600 AS VARCHAR) + ':' + 
                                    FORMAT(SUM(total_segundos) % 3600 / 60, '00') + ':' + 
                                    FORMAT(SUM(total_segundos) % 60, '00')
                             FROM (
                                SELECT SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                 CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                 ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                FROM hr_horario2 b2
                                WHERE b2.id_horario1 = a.id_horario1
                                -- SIN filtro de códigos, suma TODO
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
                                                  [RAREA], [RCOMUNI], [RORIENT], [RPERSONAL], [RTP])
                                ) pvt
                            ) piv
                            WHERE a.empleado = @employee";

                //var parameters = new DynamicParameters();
                var list = await connection.QueryFirstAsync<HorarioAuxDTO>(query, new { employee = employee });
                return list;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los horarios.");
                throw;
            }
        }

        public async Task<List<HorarioAuxDTO>> OnlyHours(string employee)
        {
            try
            {
                using var connection = _dapper.Crearconexion();
                var query = @"SELECT 
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
                                -- SIN filtro de códigos, suma TODO
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
                            WHERE a.empleado = @employee";

                //var parameters = new DynamicParameters();
                var list = await connection.QueryAsync<HorarioAuxDTO>(query, new { employee = employee });
                return list.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los horarios.");
                throw;
            }
        }

        public async Task<List<HorarioAuxDTO>> OnlyHoursByArea(string area)
        {
            try
            {
                using var connection = _dapper.Crearconexion();
                var query = @"SELECT a.empleado,
                                -- Formato h:mm:ss para IA
                                coalesce((SELECT CAST(total / 3600 AS VARCHAR) + ':' +  FORMAT(total % 3600 / 60, '00') + ':' +  FORMAT(total % 60, '00')
                                 FROM (SELECT SUM(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 + ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0)) AS total   
                                       FROM hr_horario2 a3
                                       WHERE id_horario1 = a.id_horario1 AND id_mps > 0) LL),'00:00:00') AS [IA],
                            
                                -- Columnas del PIVOT de actividades con formato h:mm:ss
                            piv.[AAA],piv.[ALM],piv.[CUIDA],piv.[FMAGIS],piv.[GAA],piv.[GAO],piv.[ORA],piv.[RAREA],piv.[RCOMUNI],piv.[RORIENT],piv.[RPERSONAL],piv.[RTP],piv.[ANP],
                            
                                -- Columna TOTAL - cálculo directo desde la tabla
                            (SELECT CAST(SUM(total_segundos) / 3600 AS VARCHAR) + ':' + 
                                    FORMAT(SUM(total_segundos) % 3600 / 60, '00') + ':' + 
                                    FORMAT(SUM(total_segundos) % 60, '00')
                             FROM (
                                SELECT SUM(ISNULL(CAST(SUBSTRING(tiempo, 1, 2) AS INT) * 3600 + 
                                                 CAST(SUBSTRING(tiempo, 4, 2) AS INT) * 60 +
                                                 ISNULL(CAST(SUBSTRING(tiempo, 7, 2) AS INT), 0), 0)) as total_segundos   
                                FROM hr_horario2 b2
                                WHERE b2.id_horario1 = a.id_horario1
                                -- SIN filtro de códigos, suma TODO
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
                            WHERE a.area = @area;";

                //var parameters = new DynamicParameters();
                var list = await connection.QueryAsync<HorarioAuxDTO>(query, new { area = area });
                return list.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los horarios.");
                throw;
            }
        }
    }
}
