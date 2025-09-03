using DevsuBackend.DTOs.Reporte;

namespace DevsuBackend.DTOs.Response
{
    public class ReporteResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public List<MovimientoReporteDto> Movimientos { get; set; }
        public ResumenReporteDto Resumen { get; set; }
        public string PdfBase64 { get; set; }
        public DateTime FechaGeneracion { get; set; }
    }
}
