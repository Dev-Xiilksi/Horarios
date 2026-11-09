namespace SistemaHorario.Application.DTOs
{
    public class HrActividadDTO
    {
        public short IdHra { get; set; }
        public string Nombre { get; set; } = null!;
        public bool? Activo { get; set; }
    }
}
