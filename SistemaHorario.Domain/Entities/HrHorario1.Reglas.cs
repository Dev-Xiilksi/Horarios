using SistemaHorario.Domain.Constants;

namespace SistemaHorario.Domain.Entities;

//reglas de negocio del horario (la otra parte de la clase es el mapeo de la tabla)
public partial class HrHorario1
{
    public bool EstaAprobado() => IdConfirmacion == EstadoConfirmacion.Aprobado;

    //el coordinador aprueba el horario del educador
    public HrBitacoraConfirmacion Aprobar(int idCoordinador)
    {
        IdConfirmacion = EstadoConfirmacion.Aprobado;

        return new HrBitacoraConfirmacion
        {
            id_horario1 = IdHorario1,
            id_coordinador = idCoordinador,
            registro_fecha = DateTime.Now.Date,
            hora_registro = DateTime.Now.TimeOfDay
        };
    }
}
