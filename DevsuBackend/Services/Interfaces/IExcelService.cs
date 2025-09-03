using DevsuBackend.DTOs.Response;

namespace DevsuBackend.Services.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> GenerarExcelReporteAsync(ReporteResponseDto reporte);
        byte[] GenerarExcelFromData<T>(List<T> data, string sheetName = "Reporte");

    }
}
