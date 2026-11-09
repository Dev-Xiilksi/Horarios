using SistemaHorario.Application.DTOs;

namespace SistemaHorario.Application.Interfaces
{
    //lado de lectura de horarios (lo usan las consultas); devuelve DTOs listos para la UI
    public interface IHorarioConsultas
    {
        Task<List<HrHorario1DTO>> ObtenerTodos(string ciclo);
        Task<List<HrHorario1DTO>> ObtenerPorEstado(short area, string? ciclo, int estado);
        Task<HrHorario1DTO?> ObtenerPorId(int id);
        Task<HrHorario1DTO?> ObtenerPorEmpleado(int empleado, string ciclo);
        Task<List<HrHorario2DTO>> ObtenerFranjas(int idHorario1);
    }
}
