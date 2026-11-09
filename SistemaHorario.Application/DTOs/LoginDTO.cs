namespace SistemaHorario.Application.DTOs
{
    public class LoginDTO
    {
        public string? IdUser { get; set; }
        public string? UserName { get; set; }
        public string? Group { get; set; }
        public bool IsAuthenticated { get; set; }
        public string? Rol { get; set; }
        public string? Token { get; set; }
        public int Area { get; set; }
        public string? AreaName { get; set; }
        public string? Message { get; set; }
    }
}
