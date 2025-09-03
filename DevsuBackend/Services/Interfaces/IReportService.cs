using DevsuBackend.DTOs.Reporte;
using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;

namespace DevsuBackend.Services.Interfaces
{
    public interface IReportService
    {
        Task<ReporteResponseDto> GenerarReporteEstadoCuentaAsync(ReporteRequestDto request);
        Task<ReporteResponseDto> GenerarReporteDetalladoAsync(ReporteFiltroDto filtros);
        Task<byte[]> GenerarPdfReporteAsync(ReporteResponseDto reporte);
        Task<byte[]> GenerarExcelReporteAsync(ReporteResponseDto reporte);
        Task<ResumenReporteDto> ObtenerResumenMovimientosAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
        Task<List<MovimientoReporteDto>> ObtenerMovimientosPorClienteAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin);
        Task<List<MovimientoReporteDto>> ObtenerMovimientosPorCuentaAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin);
    }
}
