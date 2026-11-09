using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Grado
{
    public string Colegio { get; set; } = null!;

    public string Nivel { get; set; } = null!;

    public string Grado1 { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Origen { get; set; }

    public bool? Real { get; set; }
}
