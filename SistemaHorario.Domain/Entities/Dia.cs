using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Dia
{
    public short Dia1 { get; set; }

    public string? Nombre { get; set; }

    public bool? Habil { get; set; }

    public virtual ICollection<HrHorario2> HrHorario2s { get; set; } = new List<HrHorario2>();
}
