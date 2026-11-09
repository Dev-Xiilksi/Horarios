namespace SistemaHorario.Application.DTOs
{
    public class HrGradosDto
    {
        public int IdHrGrados { get; set; }
        public int IdHorario1 { get; set; }
        public string? Colegio { get; set; }
        public string? Nivel { get; set; }
        public string? Grado { get; set; }
        public string? Seccion { get; set; }
        public string NivelGrado { get; set; } = string.Empty;
        public virtual HrHorario1DTO IdHorario1Navigation { get; set; } = null!;
    }
}
