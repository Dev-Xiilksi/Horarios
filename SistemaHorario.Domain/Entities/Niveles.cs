using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Niveles
{
    public string Colegio { get; set; } = null!;

    public string Nivel { get; set; } = null!;

    public string? Nombre { get; set; }

    public short? Orden { get; set; }

    public string? Promo { get; set; }
}
