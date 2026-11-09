namespace SistemaHorario.Application.DTOs
{
    public class ActividadDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Type { get; set; }
        public int TipoOpcion { get; set; }
    }
}
