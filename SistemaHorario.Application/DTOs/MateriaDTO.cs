namespace SistemaHorario.Application.DTOs
{
    public class MateriaDTO
    {
        public string Colegio { get; set; } = null!;
        public short Codigo { get; set; }
        public string? Nombre { get; set; }
        public short? Area { get; set; }
        public string? Nombrec { get; set; }
        public string? Tipo { get; set; }
        public string? Aspecto { get; set; }
        public short? Codpadre { get; set; }
        public bool? Ver { get; set; }
    }
}
