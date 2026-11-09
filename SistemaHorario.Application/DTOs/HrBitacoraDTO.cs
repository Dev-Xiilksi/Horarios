namespace SistemaHorario.Application.DTOs
{
    public class HrBitacoraDTO
    {
        public int IdBitacora { get; set; }
        public int? Usuario { get; set; }
        public int? IdHorario { get; set; }
        public string? Accion { get; set; }
        public string? Mensaje { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeSpan HoraRegistro { get; set; }
    }
}
