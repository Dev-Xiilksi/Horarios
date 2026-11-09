namespace SistemaHorario.Application.DTOs
{
    public class HrHorario1DTO
    {
        public int IdHorario1 { get; set; }
        public int? IdHrg1 { get; set; }
        public short? Empresa { get; set; }
        public int Empleado { get; set; }
        public string? Ciclo { get; set; }
        public int Area { get; set; }
        public string  Colegio1 { get; set; } = string.Empty;
        public string? Colegio { get; set; }
        public string Nivel { get; set; } = string.Empty;
        public List<string> Nivels { get; set; } = new List<string>();
        public string? Grado { get; set; }
        public List<string> Grados { get; set; } = new List<string>();
        public int IdConfirmacion { get; set; }
        public DateTime Fecha_Registro { get; set; } = new();
        public TimeSpan Hora_Registro { get; set; } = new();
        public int TipoFranja { get; set; }
        public virtual ICollection<HrHorario2DTO> HrHorario2s { get; set; } = new List<HrHorario2DTO>();
        public virtual ICollection<HrGradosDto> HrGrados { get; set; } = new List<HrGradosDto>();

        //
        public string? NombreGeneral { get; set; }
        public string? NombreArea { get; set; }
        public string? NombreEmpleado { get; set; }
        public int TotalPorArea { get; set; }
        //para confirmar
        public int IdCoordinador { get; set; }
        public int PoseeHorario2 { get; set; }
    }
}
