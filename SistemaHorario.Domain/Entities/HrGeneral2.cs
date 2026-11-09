using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrGeneral2
{
    public int IdHrg2 { get; set; }

    public int? IdHrg1 { get; set; }

    public string? Horai { get; set; }

    public string? Horaf { get; set; }

    public string? Tiempo { get; set; }

    public virtual ICollection<HrHorario2> HrHorario2s { get; set; } = new List<HrHorario2>();

    public virtual HrGeneral? IdHrg1Navigation { get; set; }
}
