using System.ComponentModel.DataAnnotations;

namespace DevsuBackend.DTOs.Request
{
    public class LoginRequestDto
    {
        [Required]
        public string Identificacion { get; set; }

        [Required]
        public string Contrasena { get; set; }
    }
}
