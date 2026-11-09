namespace SistemaHorario.Domain.Entities
{
    public class Rol
    {
        public int IdRol { get; set; }

        public string? Nombre { get; set; }

        public virtual ICollection<AccesoUsuario> AccesoUsuarios { get; set; } = new List<AccesoUsuario>();
    }
}
