using DB.Models;
using DevsuBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevsuBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CuentasController : ControllerBase
    {
        private readonly ICuentaService _cuentaService;
        private readonly ILogger<CuentasController> _logger;

        public CuentasController(ICuentaService cuentaService, ILogger<CuentasController> logger)
        {
            _cuentaService = cuentaService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cuenta>>> GetCuentas()
        {
            try
            {
                var cuentas = await _cuentaService.GetAllCuentasAsync();
                return Ok(cuentas);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cuentas");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Cuenta>> GetCuenta(int id)
        {
            try
            {
                var cuenta = await _cuentaService.GetCuentaByIdAsync(id);
                if (cuenta == null) return NotFound();
                return Ok(cuenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener cuenta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Cuenta>> PostCuenta(Cuenta cuenta)
        {
            try
            {
                var nuevaCuenta = await _cuentaService.CreateCuentaAsync(cuenta);
                return CreatedAtAction(nameof(GetCuenta), new { id = nuevaCuenta.CuentaId }, nuevaCuenta);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear cuenta");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutCuenta(int id, Cuenta cuenta)
        {
            try
            {
                if (id != cuenta.CuentaId) return BadRequest();

                var cuentaActualizada = await _cuentaService.UpdateCuentaAsync(id, cuenta);
                if (cuentaActualizada == null) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar cuenta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCuenta(int id)
        {
            try
            {
                var resultado = await _cuentaService.DeleteCuentaAsync(id);
                if (!resultado) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar cuenta con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
