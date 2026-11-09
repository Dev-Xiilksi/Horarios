using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class HrHorario2
{
    public int IdHorario2 { get; set; }

    public int? IdHorario1 { get; set; }

    public short? Dia { get; set; }

    public int? IdHrg2 { get; set; }

    public int? IdMps { get; set; }

    public short? IdHra { get; set; }

    public string? Horai { get; set; }

    public string? Horaf { get; set; }

    public string? Tiempo { get; set; }
    public int NumeroFila { get; set; }
    //public int? ActividadVariable { get; set; }

    public virtual Dia? DiaNavigation { get; set; }

    public virtual HrHorario1? IdHorario1Navigation { get; set; }

    public virtual HrActividades? IdHraNavigation { get; set; }

    public virtual HrGeneral2? IdHrg2Navigation { get; set; }

    public virtual Maestrosporseccion? IdMpsNavigation { get; set; }
}
