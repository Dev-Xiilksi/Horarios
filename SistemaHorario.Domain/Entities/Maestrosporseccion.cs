using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Maestrosporseccion
{
    public int? Codigo { get; set; }

    public string? Colegio { get; set; }

    public string? Nivel { get; set; }

    public string? Grado { get; set; }

    public string? Seccion { get; set; }

    public short? Codmateria { get; set; }

    public int IdMps { get; set; }

    public int? MdlgroupId { get; set; }

    public string? Ciclo { get; set; }

    public virtual ICollection<HrHorario2> HrHorario2s { get; set; } = new List<HrHorario2>();
}
