using SistemaHorario.Domain.Entities;

namespace SistemaHorario.Application.Interfaces
{
    //lado de escritura de horarios (lo usan los comandos)
    public interface IHorarioRepository
    {
        //horario para aplicar reglas de dominio; se guarda con GuardarAprobacion
        Task<HrHorario1?> Buscar(int idHorario1);
        Task<bool> ExisteEnEstado(int empleado, string ciclo, int estado);
        Task Agregar(HrHorario1 horario);
        //guarda el nuevo estado de confirmacion del horario y su registro de confirmacion, en una sola transaccion
        Task<int> GuardarAprobacion(HrHorario1 horario, HrBitacoraConfirmacion confirmacion);
        //elimina el horario con sus detalles; devuelve el id eliminado o null si no existe
        Task<int?> EliminarConDetalle(int empleado, string ciclo);
    }
}
