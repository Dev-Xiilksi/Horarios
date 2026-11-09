namespace SistemaHorario.Application.DTOs
{
    public class AreaDTO
    {
        public int Areas { get; set; }
        public string? Nombre { get; set; }
        public string? Código { get; set; }

        //aux
        public string? Ciclo { get; set; }
        public int? TotalPorArea { get; set; }
    }
}
