using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaHorario.Domain.Entities
{
    public class HrGrado
    {
        [Key]
        [Column("id_hr_grados")]
        public int IdHrGrados { get; set; }

        [Column("id_horario1")]
        public int IdHorario1 { get; set; }

        [Column("colegio")]
        [StringLength(1)]
        public string Colegio { get; set; } = string.Empty;

        [Column("nivel")]
        [StringLength(1)]
        public string Nivel { get; set; } = string.Empty;

        [Column("grado")]
        [StringLength(1)]
        public string Grado { get; set; } = string.Empty;

        [Column("seccion")]
        [StringLength(1)]
        public string Seccion { get; set; } = string.Empty;

        [Column("nivel_grado")]
        [StringLength(2)]
        public string NivelGrado { get; set; } = string.Empty;

        [ForeignKey("IdHorario1")]
        [InverseProperty("HrGrados")]
        public virtual HrHorario1 IdHorario1Navigation { get; set; } = null!;
    }
}
