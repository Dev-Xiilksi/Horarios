namespace SistemaHorario.Application.DTOs.Auxiliares
{
    public class HorarioExcelDto
    {
        public ColaboradorDTO Empleado { get; set; } = new();

        public List<HrGradosDto> Grados { get; set; } = new();

        public List<FranjasDto> Franjas { get; set; } = new();
    }
}
