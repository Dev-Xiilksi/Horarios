using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Area
{
    /// <summary>
    /// AREAS ACADEMICAS A QUE CORRESPONDEN LAS MATERIAS QUE SE IMPARTEN
    /// </summary>
    public short Area1 { get; set; }

    public string? Nombre { get; set; }

    public int? MdlcategoryId { get; set; }

    public string? Codigo { get; set; }
    public virtual ICollection<AccesoUsuario> AccesoUsuarios { get; set; } = new List<AccesoUsuario>();
}
