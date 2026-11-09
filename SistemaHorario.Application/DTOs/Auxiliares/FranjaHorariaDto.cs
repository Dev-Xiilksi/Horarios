namespace SistemaHorario.Application.DTOs.Auxiliares
{
    //fila de hr_horario2 tal como se guarda (una actividad en un dia y hora)
    public class FranjaHorariaDto
    {
        public string? Horai { get; set; }
        public string? Horaf { get; set; }
        public string? Tiempo { get; set; }
        public string Dia { get; set; } = string.Empty;
        public int? IdMps { get; set; }
        public short? IdHra { get; set; }
    }
}
