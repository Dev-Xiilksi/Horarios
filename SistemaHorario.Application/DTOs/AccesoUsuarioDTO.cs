namespace SistemaHorario.Application.DTOs
{
    public class AccesoUsuarioDTO
    {
        public int IdAcceso { get; set; }

        public int? IdRol { get; set; }

        public int IdEmpleado { get; set; }

        public int IdArea { get; set; }

        public DateTime? FechaRegistro { get; set; }
        public virtual AreaDTO? IdAreaNavigation { get; set; }

        public virtual RolDTO? IdRolNavigation { get; set; }

        //aux
        public string? FullName { get; set; }
        public virtual string? RolName { get; set;}
        public string? AreaName { get; set; }
    }
}
