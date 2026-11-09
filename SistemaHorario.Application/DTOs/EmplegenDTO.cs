namespace SistemaHorario.Application.DTOs
{
    public class EmplegenDTO
    {
        public short Empresa { get; set; }
        public int Empleado { get; set; }
        public string? FullName { get; set; } = "";
    }
}
