namespace SistemaHorario.Domain.Entities
{
    public partial class AccesoUsuario
    {
        public int IdAcceso { get; set; }

        public int? IdRol { get; set; }

        public int IdEmpleado { get; set; }

        public short IdArea { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual Area? IdAreaNavigation { get; set; }

        public virtual Rol? IdRolNavigation { get; set; }
    }
}
