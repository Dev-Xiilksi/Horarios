namespace SistemaHorario.Application.DTOs
{
    public class CorreoDTO
    {
        public int IdCorreo { get; set; }
        public int Area { get; set; }
        public int EncargadoArea { get; set; }
        public string? CorreoEncargado { get; set; }
        public DateTime FechaRegistro { get; set; }
        public TimeSpan HoraRegistro { get; set; }
    }
}
