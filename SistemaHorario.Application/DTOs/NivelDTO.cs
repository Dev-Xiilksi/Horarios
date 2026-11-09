namespace SistemaHorario.Application.DTOs
{
    public class NivelDTO
    {
        public string Colegio { get; set; } = null!;
        public string Nivel { get; set; } = null!;
        public string? Nombre { get; set; }
        public short? Orden { get; set; }
        public string? Promo { get; set; }
    }
}
