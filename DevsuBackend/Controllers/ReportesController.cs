using DevsuBackend.DTOs.Request;
using DevsuBackend.DTOs.Response;
using DevsuBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevsuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportesController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(IReportService reportService, ILogger<ReportesController> logger)
        {
            _reportService = reportService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<ReporteResponseDto>> GetReporte([FromQuery] ReporteRequestDto request)
        {
            try
            {
                var reporte = await _reportService.GenerarReporteEstadoCuentaAsync(request);
                return Ok(reporte);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar reporte");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
