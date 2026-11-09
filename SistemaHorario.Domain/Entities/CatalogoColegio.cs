namespace SistemaHorario.Domain.Entities
{
    public class CatalogoColegio
    {
        public int Id { get; set; }
        public string? Colegio { get; set; }
        public string? Nivel { get; set; }
        public string? Grado { get; set; }
        public string? Seccion { get; set; }
        public string? Unidad { get; set; }
        public string? Nombre { get; set; }
        public short? Unidades { get; set; }
        public short? Notaoficial { get; set; }
        public short? Notacolegio { get; set; }
        public short? Numrecupera { get; set; }
        public string? Nomnivel { get; set; }
        public string? Nomgrado { get; set; }
    }
}
