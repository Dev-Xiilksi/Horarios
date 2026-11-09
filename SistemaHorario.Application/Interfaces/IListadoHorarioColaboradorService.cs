using SistemaHorario.Application.DTOs.Auxiliares;

namespace SistemaHorario.Application.Interfaces
{
    public interface IListadoHorarioColaboradorService
    {
        Task<List<ColaboradorDTO>> ListaColaboradores();
        Task<List<ColaboradorDTO>> ListaColaboradoresVariables();
        Task<List<EventoDTO>> ListaEventos(int idHorario);
        Task<List<ActividadAuxDTO>> ListaActividades(int idHorario);
        Task<List<ColaboradorDTO>> Colaborador(int idempleado);
    }
}
