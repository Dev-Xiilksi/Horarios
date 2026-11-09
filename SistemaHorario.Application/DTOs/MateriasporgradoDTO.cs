namespace SistemaHorario.Application.DTOs
{
    public class MateriasporgradoDTO
    {
        public string Ciclo { get; set; } = null!;
        public string Colegio { get; set; } = null!;
        public string Nivel { get; set; } = null!;
        public string Grado { get; set; } = null!;
        public short Codigo { get; set; }
        public short? Orden { get; set; }
        public int? MdlcourseId { get; set; }
        public int IdMg { get; set; }
        public bool? Visible { get; set; }
        public string? Promedio { get; set; }
    }
}
