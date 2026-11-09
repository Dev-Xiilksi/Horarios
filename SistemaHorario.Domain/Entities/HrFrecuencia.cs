using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrFrecuencia
{
    public short IdHrf { get; set; }

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }

    public virtual ICollection<HrHorarioExtra> HrHorarioExtras { get; set; } = new List<HrHorarioExtra>();
}
