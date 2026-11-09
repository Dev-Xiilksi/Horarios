namespace SistemaHorario.Application.DTOs
{
    public class HrLogErrorsDTO
    {
        public int Id { get; set; }
        public string? Mensaje { get; set; }
        public string? StackTrace { get; set; }
        public string? Fuente { get; set; }
        public string? Metodo { get; set; }
        public string? Usuario { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Hora { get; set; }
    }
}
