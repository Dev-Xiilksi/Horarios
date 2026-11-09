using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrHorario1
{
    public int IdHorario1 { get; set; }

    public int? IdHrg1 { get; set; }

    public short? Empresa { get; set; }

    public int Empleado { get; set; }

    public string? Ciclo { get; set; }

    public short? Area { get; set; }
    public string? Colegio { get; set; }

    public string? Nivel { get; set; }

    public string? Grado { get; set; }

    public string? Seccion { get; set; }

    public string? CorrecionTrabajos { get; set; }

    public short? Estudiantes { get; set; }

    public string? Observacion { get; set; }
    public int IdConfirmacion { get; set; }
    public DateTime Fecha_Registro { get; set; } = new();
    public TimeSpan Hora_Registro { get; set; } = new();
    public int TipoFranja { get; set; }

    public virtual Emplegen? Emplegen { get; set; }
    public virtual ICollection<HrGrado> HrGrados { get; set; } = new List<HrGrado>();
    public virtual ICollection<HrHorario2> HrHorario2s { get; set; } = new List<HrHorario2>();

    public virtual ICollection<HrHorarioExtra> HrHorarioExtras { get; set; } = new List<HrHorarioExtra>();

    public virtual HrGeneral? IdHrg1Navigation { get; set; }
}
