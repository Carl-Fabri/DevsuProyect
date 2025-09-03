using DevsuBackend.DTOs.Response;

namespace DevsuBackend.Services.Interfaces
{
    public interface IPdfService
    {
        Task<byte[]> GenerarPdfEstadoCuentaAsync(ReporteResponseDto reporte);
        byte[] GenerarPdfFromHtml(string htmlContent);
        string GenerarHtmlReporte(ReporteResponseDto reporte);
    }
}
