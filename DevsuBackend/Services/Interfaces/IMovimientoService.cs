using DB.Models;

namespace DevsuBackend.Services.Interfaces
{
    public interface IMovimientoService
    {
        Task<IEnumerable<Movimiento>> GetAllMovimientosAsync();
        Task<Movimiento> GetMovimientoByIdAsync(int id);
        Task<Movimiento> CreateMovimientoAsync(Movimiento movimiento);
        Task<Movimiento> UpdateMovimientoAsync(int id, Movimiento movimiento);
        Task<bool> DeleteMovimientoAsync(int id);
        Task<bool> MovimientoExistsAsync(int id);
        Task<decimal> GetTotalRetirosHoyAsync(int cuentaId);
        Task<IEnumerable<Movimiento>> GetMovimientosPorFechaAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin);
    }
}
