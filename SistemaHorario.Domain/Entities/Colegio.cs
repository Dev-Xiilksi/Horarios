using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Colegio
{
    public string Colegio1 { get; set; } = null!;

    public string? Nombre { get; set; }

    public short? Empresa { get; set; }

    public short? Notaoficial { get; set; }

    public short? Notacolegio { get; set; }
}
