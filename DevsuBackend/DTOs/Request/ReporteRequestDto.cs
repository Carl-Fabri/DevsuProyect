using System.ComponentModel.DataAnnotations;

namespace DevsuBackend.DTOs.Request
{
    public class ReporteRequestDto
    {
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El ID del cliente es requerido")]
        [Range(1, int.MaxValue, ErrorMessage = "El ID del cliente debe ser válido")]
        public int ClienteId { get; set; }

        public string Formato { get; set; } = "json"; // json, pdf, ambos
    }
}
