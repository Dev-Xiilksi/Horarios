using Microsoft.Extensions.Logging;
using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Application.Seguridad;
using SistemaHorario.Domain.ValueObjects;

namespace SistemaHorario.Application.Services
{
    public class ReporteService : IReporteService
    {
        private readonly IReporteRepository _repository;
        private readonly IHorarioConsultas _horarios;
        private readonly IExcelHorarioService _excel;
        private readonly IUsuarioActual _usuarioActual;
        private readonly ILogger<ReporteService> _logger;

        public ReporteService(IReporteRepository repository, IHorarioConsultas horarios, IExcelHorarioService excel, IUsuarioActual usuarioActual, ILogger<ReporteService> logger)
        {
            _repository = repository;
            _horarios = horarios;
            _excel = excel;
            _usuarioActual = usuarioActual;
            _logger = logger;
        }

        //area 0 = todas las areas
        public async Task<byte[]> GenerateExcelWithCustomHeadersAsync(AreaDTO obj)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var permitido = obj.Areas == 0 ? ReglasDeAcceso.VeTodo(usuario) : ReglasDeAcceso.PuedeVerArea(usuario, obj.Areas);
            if (!permitido)
                throw new AccesoDenegadoException("No tiene permiso para exportar los horarios de esta área.");

            var list = await _repository.ObtenerDetalleParaExportar(obj.Ciclo, obj.Areas);
            return _excel.GenerarExcelResumen(list);
        }

        public async Task<List<HorarioAuxDTO>> ObtenerHorariosResumen(string ciclo)
        {
            await ExigirVisionGlobal();
            try
            {
                return await _repository.ObtenerHorariosResumen(ciclo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los datos.");
                throw;
            }
        }

        public async Task<HorarioExcelDto> ObtenerExcel(int id)
        {
            string ciclo = CicloEscolar.Actual();

            //el permiso depende del area del horario del empleado
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            var horario = await _horarios.ObtenerPorEmpleado(id, ciclo);
            if (horario != null && !ReglasDeAcceso.PuedeVerHorario(usuario, horario.Empleado, horario.Area))
                throw new AccesoDenegadoException("No tiene permiso para ver este horario.");

            var empleado = await _repository.ObtenerColaborador(id, ciclo)
                ?? throw new InvalidOperationException($"No se encontró horario del empleado {id} en el ciclo {ciclo}.");

            var grados = await _repository.ObtenerGrados(empleado.IdHorario);
            var materias = await _repository.ObtenerMateriasDelMaestro(empleado.Codigo, ciclo);
            var actividades = await _repository.ObtenerActividades();
            var filas = await _repository.ObtenerFranjas(empleado.IdHorario);

            return new HorarioExcelDto
            {
                Empleado = empleado,
                Grados = grados,
                Franjas = HorarioSemanalBuilder.Construir(filas, materias, actividades)
            };
        }

        public async Task<List<HorarioConsolidadoDto>> ObtenerConsolidadoPorArea(string area, string ciclo)
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            if (!short.TryParse(area, out var idArea) || !ReglasDeAcceso.PuedeVerArea(usuario, idArea))
                throw new AccesoDenegadoException("No tiene permiso para ver el consolidado de esta área.");

            try
            {
                return await _repository.ObtenerConsolidados(ciclo, area);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los datos.");
                throw;
            }
        }

        public async Task<List<HorarioConsolidadoDto>> ObtenerConsolidados(string ciclo)
        {
            await ExigirVisionGlobal();
            try
            {
                return await _repository.ObtenerConsolidados(ciclo, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los datos.");
                throw;
            }
        }

        //reportes de todas las areas: administrador, consultor o coordinador de un area con vision global
        private async Task ExigirVisionGlobal()
        {
            var usuario = await AccesoDenegadoException.ExigirSesion(_usuarioActual);
            if (!ReglasDeAcceso.VeTodo(usuario))
                throw new AccesoDenegadoException("No tiene permiso para ver los reportes de todas las áreas.");
        }
    }
}
