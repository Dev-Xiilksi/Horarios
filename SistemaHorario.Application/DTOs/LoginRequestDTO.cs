using System.ComponentModel.DataAnnotations;

namespace SistemaHorario.Application.DTOs
{
    public class LoginRequestDTO
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "La contraseña es requerida.")]
        public string? Password { get; set; }
    }
}
