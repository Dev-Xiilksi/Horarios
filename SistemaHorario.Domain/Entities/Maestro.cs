namespace SistemaHorario.Domain.Entities
{
    public partial class Maestro
    {
        public string? Colegio { get; set; }

        public int Codigo { get; set; }

        public string? Apell1 { get; set; }

        public string? Apell2 { get; set; }

        public string? Nom1 { get; set; }

        public string? Nom2 { get; set; }

        public short? Area { get; set; }

        public int Empleado { get; set; }

        public string? Tipo { get; set; }

        public string? Tipomaestro { get; set; }

        public string? Correo { get; set; }

        public string? Username { get; set; }

        public int? MdluserId { get; set; }
    }
}
