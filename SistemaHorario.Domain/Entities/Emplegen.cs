using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Emplegen
{
    public short Empresa { get; set; }

    public int Empleado { get; set; }

    public string? Apellido1 { get; set; }

    public string? Apellido2 { get; set; }

    public string? Casada { get; set; }

    public string? Nombre1 { get; set; }

    public string? Nombre2 { get; set; }

    public string? Nombre3 { get; set; }

    public string? Direccion { get; set; }

    public string? Calle { get; set; }

    public string? Numero { get; set; }

    public string? Apto { get; set; }

    public short? Zona { get; set; }

    public string? Colonia { get; set; }

    public short? Deptodir { get; set; }

    public short? Munidir { get; set; }

    public string? Telefono { get; set; }

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public short? Nacional { get; set; }

    public string? Etnia { get; set; }

    public DateTime? Fechanac { get; set; }

    public short? Tipoiden { get; set; }

    public string? Registro { get; set; }

    public string? Numidentica { get; set; }

    public short? Deptoced { get; set; }

    public short? Municed { get; set; }

    public string? Docdocente { get; set; }

    public string? Civil { get; set; }

    public string? Sexo { get; set; }

    public string? Numseguro { get; set; }

    public string? Nit { get; set; }

    public short? Economica { get; set; }

    public DateTime? Fechai { get; set; }

    public string? Usuario { get; set; }

    public string? Correoi { get; set; }

    public int? IdArea { get; set; }

    public short? IdNivelEducativo { get; set; }

    public short? IdPuebloPertenencia { get; set; }

    public short? IdComunidadLing { get; set; }

    public string? TituloPrincipal { get; set; }

    public short? IdTipoDiscapacidad { get; set; }

    public virtual ICollection<Contratos1> Contratos1s { get; set; } = new List<Contratos1>();

    public virtual Empresa EmpresaNavigation { get; set; } = null!;

    public virtual ICollection<HrHorario1> HrHorario1s { get; set; } = new List<HrHorario1>();
}
