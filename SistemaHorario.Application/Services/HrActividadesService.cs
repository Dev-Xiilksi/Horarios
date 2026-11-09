using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.Interfaces;
using SistemaHorario.Domain.ValueObjects;

namespace SistemaHorario.Application.Services
{
    public class HrActividadesService : IHrActividadesService
    {
        private const short Empresa = 1;

        private readonly IActividadRepository _repository;

        public HrActividadesService(IActividadRepository repository)
        {
            _repository = repository;
        }

        //para colaboradores con horario fijo: las clases que imparte el educador y luego las actividades
        public async Task<List<ActividadDTO>> GetAll(int empleado)
        {
            var materias = await _repository.ObtenerMateriasImpartidas(empleado, CicloEscolar.Actual(), Empresa);

            var clases = materias.Select(a => new ActividadDTO
            {
                Id = Convert.ToInt16(a.IdMps),
                Name = $"Imparte a {a.Colegio}{a.Nivel}{a.Grado}{a.Seccion} {a.NombreMateria?.ToLower()}",
                Description = a.NombreMateria,
                Type = "Clase",
                TipoOpcion = 1
            });

            var actividades = (await _repository.ObtenerActividades()).OrderBy(a => a.Name);

            return clases.Concat(actividades).ToList();
        }

        //para colaboradores con horario variable
        public async Task<List<ActividadDTO>> GetAllVariableActivities()
        {
            return await _repository.ObtenerSeccionesDelColegio();
        }
    }
}
