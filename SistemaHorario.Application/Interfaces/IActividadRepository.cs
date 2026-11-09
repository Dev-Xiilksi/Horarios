using SistemaHorario.Application.DTOs;
using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IActividadRepository
    {
        Task<List<MateriaImpartidaDto>> ObtenerMateriasImpartidas(int empleado, string ciclo, short empresa);
        Task<List<ActividadDTO>> ObtenerActividades();
        Task<List<ActividadDTO>> ObtenerSeccionesDelColegio();
    }
}
