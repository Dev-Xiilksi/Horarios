namespace SistemaHorario.Application.DTOs
{
    public class RolDTO
    {
        public int IdRol { get; set; }

        public string? Nombre { get; set; }

        public virtual ICollection<AccesoUsuarioDTO> AccesoUsuarios { get; set; } = new List<AccesoUsuarioDTO>();
    }
}
