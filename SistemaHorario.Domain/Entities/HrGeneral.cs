using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrGeneral
{
    public int IdHrg1 { get; set; }

    public string? Nombre { get; set; }

    public string? Ciclo { get; set; }

    public virtual ICollection<HrGeneral2> HrGeneral2s { get; set; } = new List<HrGeneral2>();

    public virtual ICollection<HrHorario1> HrHorario1s { get; set; } = new List<HrHorario1>();
}
