namespace SistemaHorario.Application.DTOs.Auxiliares
{
    public class FranjasDto
    {
        public int Franja { get; set; }
        public string HoraInicio { get; set; } = string.Empty;
        public string HoraFin { get; set; } = string.Empty;
        public string Tiempo { get; set; } = string.Empty;
        public string Lunes { get; set; } = string.Empty;
        public string Martes { get; set; } = string.Empty;
        public string Miercoles { get; set; } = string.Empty;
        public string Jueves { get; set; } = string.Empty;
        public string Viernes { get; set; } = string.Empty;
        public string Sabados { get; set; } = string.Empty;
    }
}
