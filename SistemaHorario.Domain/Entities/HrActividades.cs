using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrActividades
{
    public short IdHra { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Descripcion { get; set; }

    public bool? Activo { get; set; }
    public int IdTipoOpcion { get; set; }

    public virtual ICollection<HrHorario2> HrHorario2s { get; set; } = new List<HrHorario2>();
}
