using DB.Models;
using DevsuBackend.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevsuBackend.Controllers
{
    // Controllers/MovimientosController.cs
    [ApiController]
    [Route("api/[controller]")]
    public class MovimientosController : ControllerBase
    {
        private readonly IMovimientoService _movimientoService;
        private readonly ILogger<MovimientosController> _logger;

        public MovimientosController(IMovimientoService movimientoService, ILogger<MovimientosController> logger)
        {
            _movimientoService = movimientoService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Movimiento>>> GetMovimientos()
        {
            try
            {
                var movimientos = await _movimientoService.GetAllMovimientosAsync();
                return Ok(movimientos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimientos");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Movimiento>> GetMovimiento(int id)
        {
            try
            {
                var movimiento = await _movimientoService.GetMovimientoByIdAsync(id);
                if (movimiento == null) return NotFound();
                return Ok(movimiento);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener movimiento con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Movimiento>> PostMovimiento(Movimiento movimiento)
        {
            try
            {
                var nuevoMovimiento = await _movimientoService.CreateMovimientoAsync(movimiento);
                return CreatedAtAction(nameof(GetMovimiento), new { id = nuevoMovimiento.MovimientoId }, nuevoMovimiento);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear movimiento");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovimiento(int id, Movimiento movimiento)
        {
            try
            {
                if (id != movimiento.MovimientoId) return BadRequest();

                var movimientoActualizado = await _movimientoService.UpdateMovimientoAsync(id, movimiento);
                if (movimientoActualizado == null) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar movimiento con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMovimiento(int id)
        {
            try
            {
                var resultado = await _movimientoService.DeleteMovimientoAsync(id);
                if (!resultado) return NotFound();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar movimiento con ID {Id}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
