using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrHorarioExtra
{
    public int IdHrEx { get; set; }

    public int? IdHorario1 { get; set; }

    public short? IdHex { get; set; }

    public short? IdHrf { get; set; }

    public virtual HrEvento? IdHexNavigation { get; set; }

    public virtual HrHorario1? IdHorario1Navigation { get; set; }

    public virtual HrFrecuencia? IdHrfNavigation { get; set; }
}
