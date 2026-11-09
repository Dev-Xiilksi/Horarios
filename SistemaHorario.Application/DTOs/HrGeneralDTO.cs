namespace SistemaHorario.Application.DTOs
{
    public class HrGeneralDTO
    {
        public int IdHrg1 { get; set; }
        public string? Nombre { get; set; }
        public string? Ciclo { get; set; }
        public ICollection<HrGeneral2DTO> HrGeneral2 { get; set; } = new List<HrGeneral2DTO>();

        public string? FullValue { get; set; }
        //public string? Horai { get; set; }
        //public string? Horaf { get; set; }
        //public string? TiempoTotal { get; set; }
    }
}
