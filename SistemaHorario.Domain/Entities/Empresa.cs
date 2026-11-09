using System;
using System.Collections.Generic;

namespace SistemaHorario.Domain.Entities;

public partial class Empresa
{
    public short Empresa1 { get; set; }

    public string? Nombre { get; set; }

    public string? Nombre2 { get; set; }

    public string? Direccion { get; set; }

    public short? Departamento { get; set; }

    public short? Municipio { get; set; }

    public string? Telefono { get; set; }

    public string? Nit { get; set; }

    public string? Representante { get; set; }

    public string? Contador { get; set; }

    public string? Igss { get; set; }

    public int? Proveedor { get; set; }

    public int? Provision { get; set; }

    public int? Poliza { get; set; }

    public int? Ajuste { get; set; }

    public int? Caja { get; set; }

    public int? Activo { get; set; }

    public int? Depo { get; set; }

    public int? Rete { get; set; }

    public int? Numcons { get; set; }

    public int? Recibo { get; set; }

    public int? Corte { get; set; }

    public int? Recha { get; set; }

    public int? Redi { get; set; }

    public int? Reti { get; set; }

    public int? Nota { get; set; }

    public int? Fam { get; set; }

    public int? Reticol { get; set; }

    public int? Boleta { get; set; }

    public int? Alu { get; set; }

    public int? Insbus { get; set; }

    public int? Rescate { get; set; }

    public int Notad { get; set; }

    public int Notacta { get; set; }

    public string? Moneda { get; set; }

    public int? Tarjeta { get; set; }

    public string? Mesifiscal { get; set; }

    public string? Mesffiscal { get; set; }

    public int? Debito { get; set; }

    public decimal? Iva { get; set; }

    public string? Exenta { get; set; }

    public int? Ingreso { get; set; }

    public int? Salida { get; set; }

    public string? Cuentaid { get; set; }

    public string? Cuentaih { get; set; }

    public int? Orden { get; set; }

    public int? Pedido { get; set; }

    public int? Vale { get; set; }

    public string? Serie { get; set; }

    public int? Factura { get; set; }

    public string? Ctacobrar { get; set; }

    public string? Correo { get; set; }

    public string? PaginaWeb { get; set; }

    public int? FacturaExenta { get; set; }

    public string? SerieExenta { get; set; }

    public int? FacturaIsr { get; set; }

    public string? SerieIsr { get; set; }

    public virtual ICollection<Emplegen> Emplegens { get; set; } = new List<Emplegen>();
}
