namespace SistemaHorario.Domain.Entities
{
    public class HrBitacoraConfirmacion
    {
        public int id_bitacora_confirmacion { get; set; }
        public int id_horario1 { get; set; }
        public int id_coordinador { get; set; }
        public DateTime registro_fecha { get; set; }
        public TimeSpan? hora_registro { get; set; }
    }
}
