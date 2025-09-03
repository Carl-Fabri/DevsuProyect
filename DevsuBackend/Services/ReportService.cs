using DB;
using DevsuBackend.DTOs.Reporte;
using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;
using DevsuBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DevsuBackend.Services
{
    public class ReportService : IReportService
    {
        private readonly DevsuContext _context;
        private readonly ILogger<ReportService> _logger;
        private readonly IPdfService _pdfService;
        private readonly IExcelService _excelService;

        public ReportService(DevsuContext context, ILogger<ReportService> logger,
                           IPdfService pdfService, IExcelService excelService)
        {
            _context = context;
            _logger = logger;
            _pdfService = pdfService;
            _excelService = excelService;
        }

        public async Task<ReporteResponseDto> GenerarReporteEstadoCuentaAsync(ReporteRequestDto request)
        {
            try
            {
                if (request.FechaInicio > request.FechaFin)
                    throw new ArgumentException("La fecha de inicio no puede ser mayor a la fecha de fin");

                if ((request.FechaFin - request.FechaInicio).Days > 365)
                    throw new ArgumentException("El período máximo de reporte es de 1 año");

                var movimientos = await ObtenerMovimientosPorClienteAsync(
                    request.ClienteId, request.FechaInicio, request.FechaFin);

                var resumen = await ObtenerResumenMovimientosAsync(
                    request.ClienteId, request.FechaInicio, request.FechaFin);

                string? pdfBase64 = null;
                if (request.Formato?.ToLower() == "pdf" || request.Formato?.ToLower() == "ambos")
                {
                    var reporteResponse = new ReporteResponseDto
                    {
                        Movimientos = movimientos,
                        Resumen = resumen,
                        Success = true,
                        Message = "Reporte generado exitosamente"
                    };

                    var pdfBytes = await _pdfService.GenerarPdfEstadoCuentaAsync(reporteResponse);
                    pdfBase64 = Convert.ToBase64String(pdfBytes);
                }

                return new ReporteResponseDto
                {
                    Success = true,
                    Message = "Reporte generado exitosamente",
                    Movimientos = movimientos,
                    Resumen = resumen,
                    PdfBase64 = pdfBase64,
                    FechaGeneracion = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte de estado de cuenta");
                return new ReporteResponseDto
                {
                    Success = false,
                    Message = $"Error al generar reporte: {ex.Message}"
                };
            }
        }

        public async Task<ReporteResponseDto> GenerarReporteDetalladoAsync(ReporteFiltroDto filtros)
        {
            try
            {
                var query = _context.Movimientos
                    .Include(m => m.Cuenta)
                    .ThenInclude(c => c.Cliente)
                    .AsQueryable();

                if (filtros.FechaInicio.HasValue)
                    query = query.Where(m => m.Fecha >= filtros.FechaInicio.Value);
                if (filtros.FechaFin.HasValue)
                    query = query.Where(m => m.Fecha <= filtros.FechaFin.Value);
                if (filtros.ClienteId.HasValue)
                    query = query.Where(m => m.Cuenta.ClienteId == filtros.ClienteId.Value);
                if (filtros.CuentaId.HasValue)
                    query = query.Where(m => m.CuentaId == filtros.CuentaId.Value);
                if (!string.IsNullOrEmpty(filtros.TipoMovimiento))
                    query = query.Where(m => m.TipoMovimiento == filtros.TipoMovimiento);
                if (filtros.MontoMinimo.HasValue)
                    query = query.Where(m => Math.Abs(m.Valor) >= filtros.MontoMinimo.Value);
                if (filtros.MontoMaximo.HasValue)
                    query = query.Where(m => Math.Abs(m.Valor) <= filtros.MontoMaximo.Value);

                var movimientos = await query.OrderBy(m => m.Fecha).ToListAsync();

                var movimientosDto = movimientos.Select(m => new MovimientoReporteDto
                {
                    Fecha = m.Fecha,
                    Cliente = m.Cuenta.Cliente.Nombre,
                    NumeroCuenta = m.Cuenta.NumeroCuenta,
                    TipoCuenta = m.Cuenta.TipoCuenta,
                    SaldoInicial = m.Cuenta.SaldoInicial - m.Valor,
                    EstadoCuenta = m.Cuenta.Estado,
                    Movimiento = m.Valor,
                    SaldoDisponible = m.Saldo,
                    TipoMovimiento = m.TipoMovimiento
                }).ToList();

                var resumen = new ResumenReporteDto
                {
                    TotalCreditos = movimientos.Where(m => m.Valor > 0).Sum(m => m.Valor),
                    TotalDebitos = Math.Abs(movimientos.Where(m => m.Valor < 0).Sum(m => m.Valor)),
                    TotalMovimientos = movimientos.Count,
                    CuentasInvolucradas = movimientos.Select(m => m.CuentaId).Distinct().Count(),
                    PeriodoInicio = filtros.FechaInicio ?? movimientos.Min(m => m.Fecha),
                    PeriodoFin = filtros.FechaFin ?? movimientos.Max(m => m.Fecha),
                    SaldoFinal = movimientos.LastOrDefault()?.Saldo ?? 0
                };

                return new ReporteResponseDto
                {
                    Success = true,
                    Message = "Reporte detallado generado exitosamente",
                    Movimientos = movimientosDto,
                    Resumen = resumen,
                    FechaGeneracion = DateTime.Now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte detallado");
                return new ReporteResponseDto
                {
                    Success = false,
                    Message = $"Error al generar reporte detallado: {ex.Message}"
                };
            }
        }

        public async Task<ResumenReporteDto> ObtenerResumenMovimientosAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Cuenta)
                .Where(m => m.Cuenta.ClienteId == clienteId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .ToListAsync();

            return new ResumenReporteDto
            {
                TotalCreditos = movimientos.Where(m => m.Valor > 0).Sum(m => m.Valor),
                TotalDebitos = Math.Abs(movimientos.Where(m => m.Valor < 0).Sum(m => m.Valor)),
                TotalMovimientos = movimientos.Count,
                CuentasInvolucradas = movimientos.Select(m => m.CuentaId).Distinct().Count(),
                PeriodoInicio = fechaInicio,
                PeriodoFin = fechaFin,
                SaldoFinal = movimientos.LastOrDefault()?.Saldo ?? 0
            };
        }

        public async Task<List<MovimientoReporteDto>> ObtenerMovimientosPorClienteAsync(int clienteId, DateTime fechaInicio, DateTime fechaFin)
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Cuenta)
                .ThenInclude(c => c.Cliente)
                .Where(m => m.Cuenta.ClienteId == clienteId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            return movimientos.Select(m => new MovimientoReporteDto
            {
                Fecha = m.Fecha,
                Cliente = m.Cuenta.Cliente.Nombre,
                NumeroCuenta = m.Cuenta.NumeroCuenta,
                TipoCuenta = m.Cuenta.TipoCuenta,
                SaldoInicial = m.Cuenta.SaldoInicial - m.Valor,
                EstadoCuenta = m.Cuenta.Estado,
                Movimiento = m.Valor,
                SaldoDisponible = m.Saldo,
                TipoMovimiento = m.TipoMovimiento
            }).ToList();
        }

        public async Task<List<MovimientoReporteDto>> ObtenerMovimientosPorCuentaAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            var movimientos = await _context.Movimientos
                .Include(m => m.Cuenta)
                .ThenInclude(c => c.Cliente)
                .Where(m => m.CuentaId == cuentaId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .OrderBy(m => m.Fecha)
                .ToListAsync();

            return movimientos.Select(m => new MovimientoReporteDto
            {
                Fecha = m.Fecha,
                Cliente = m.Cuenta.Cliente.Nombre,
                NumeroCuenta = m.Cuenta.NumeroCuenta,
                TipoCuenta = m.Cuenta.TipoCuenta,
                SaldoInicial = m.Cuenta.SaldoInicial - m.Valor,
                EstadoCuenta = m.Cuenta.Estado,
                Movimiento = m.Valor,
                SaldoDisponible = m.Saldo,
                TipoMovimiento = m.TipoMovimiento
            }).ToList();
        }

        public async Task<byte[]> GenerarPdfReporteAsync(ReporteResponseDto reporte)
        {
            return await _pdfService.GenerarPdfEstadoCuentaAsync(reporte);
        }

        public async Task<byte[]> GenerarExcelReporteAsync(ReporteResponseDto reporte)
        {
            return await _excelService.GenerarExcelReporteAsync(reporte);
        }
    }
}
