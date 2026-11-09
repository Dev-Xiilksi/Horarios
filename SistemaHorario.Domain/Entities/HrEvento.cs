using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrEvento
{
    public short IdHex { get; set; }

    public string? Nombre { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<HrHorarioExtra> HrHorarioExtras { get; set; } = new List<HrHorarioExtra>();
}
