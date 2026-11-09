using System.ComponentModel.DataAnnotations;
namespace SistemaHorario.Application.DTOs
{
    public class HrGeneral2DTO
    {
        public int? IdHrg2 { get; set; }
        public int? IdHrg1 { get; set; }
        public string? Horai { get; set; }
        public string? Horaf { get; set; }
        public string? Tiempo { get; set; }
        public string? FullValue { get; set; }

        //temp properties
        public int Index { get; set; }

        [Required(ErrorMessage = "Es requerido selección.")]
        public int? LunesActividadId { get; set; }
        
        [Required(ErrorMessage = "Es requerido selección.")]
        public int? MartesActividadId { get; set; }
        
        [Required(ErrorMessage = "Es requerido selección.")]
        public int? MiercolesActividadId { get; set; }
        
        [Required(ErrorMessage = "Es requerido selección.")]
        public int? JuevesActividadId { get; set; }

        [Required(ErrorMessage = "Es requerido selección.")]
        public int? ViernesActividadId { get; set; }
        [Required(ErrorMessage = "Es requerido selección.")]
        public int? SabadoActividadId { get; set; }
    }
}
