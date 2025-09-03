using DB;
using DB.Models;
using DevsuBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DevsuBackend.Services
{
    public class MovimientoService : IMovimientoService
    {
        private readonly DevsuContext _context;
        private readonly ILogger<MovimientoService> _logger;
        private readonly ICuentaService _cuentaService;
        private const decimal LIMITE_DIARIO_RETIRO = 1000;

        public MovimientoService(DevsuContext context, ILogger<MovimientoService> logger, ICuentaService cuentaService)
        {
            _context = context;
            _logger = logger;
            _cuentaService = cuentaService;
        }

        public async Task<IEnumerable<Movimiento>> GetAllMovimientosAsync()
        {
            return await _context.Movimientos
                .Include(m => m.Cuenta)
                .ThenInclude(c => c.Cliente)
                .ToListAsync();
        }

        public async Task<Movimiento> GetMovimientoByIdAsync(int id)
        {
            return await _context.Movimientos
                .Include(m => m.Cuenta)
                .ThenInclude(c => c.Cliente)
                .FirstOrDefaultAsync(m => m.MovimientoId == id);
        }

        public async Task<Movimiento> CreateMovimientoAsync(Movimiento movimiento)
        {
            var cuenta = await _cuentaService.GetCuentaByIdAsync(movimiento.CuentaId);
            if (cuenta == null)
                throw new ArgumentException("Cuenta no existe");

            // Validaciones de negocio
            if (movimiento.TipoMovimiento == "Retiro")
            {
                // Validar saldo disponible
                if (cuenta.SaldoInicial < movimiento.Valor)
                    throw new InvalidOperationException("Saldo no disponible");

                // Validar límite diario de retiro
                var retirosHoy = await GetTotalRetirosHoyAsync(movimiento.CuentaId);
                if (retirosHoy + movimiento.Valor > LIMITE_DIARIO_RETIRO)
                    throw new InvalidOperationException("Cupo diario Excedido");
            }

            // Actualizar saldo
            movimiento.Valor = movimiento.TipoMovimiento == "Deposito"
                ? Math.Abs(movimiento.Valor)
                : -Math.Abs(movimiento.Valor);

            movimiento.Saldo = cuenta.SaldoInicial + movimiento.Valor;
            movimiento.Fecha = DateTime.Now;

            // Actualizar saldo de la cuenta
            cuenta.SaldoInicial = movimiento.Saldo;

            _context.Movimientos.Add(movimiento);
            await _context.SaveChangesAsync();

            return movimiento;
        }

        public async Task<Movimiento> UpdateMovimientoAsync(int id, Movimiento movimiento)
        {
            var existingMovimiento = await _context.Movimientos.FindAsync(id);
            if (existingMovimiento == null)
                return null;

            existingMovimiento.Fecha = movimiento.Fecha;
            existingMovimiento.TipoMovimiento = movimiento.TipoMovimiento;
            existingMovimiento.Valor = movimiento.Valor;
            existingMovimiento.Saldo = movimiento.Saldo;
            existingMovimiento.CuentaId = movimiento.CuentaId;

            await _context.SaveChangesAsync();
            return existingMovimiento;
        }

        public async Task<bool> DeleteMovimientoAsync(int id)
        {
            var movimiento = await _context.Movimientos.FindAsync(id);
            if (movimiento == null)
                return false;

            _context.Movimientos.Remove(movimiento);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MovimientoExistsAsync(int id)
        {
            return await _context.Movimientos.AnyAsync(e => e.MovimientoId == id);
        }

        public async Task<decimal> GetTotalRetirosHoyAsync(int cuentaId)
        {
            return await _context.Movimientos
                .Where(m => m.CuentaId == cuentaId &&
                           m.TipoMovimiento == "Retiro" &&
                           m.Fecha.Date == DateTime.Today)
                .SumAsync(m => Math.Abs(m.Valor));
        }

        public async Task<IEnumerable<Movimiento>> GetMovimientosPorFechaAsync(int cuentaId, DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Movimientos
                .Where(m => m.CuentaId == cuentaId &&
                           m.Fecha >= fechaInicio &&
                           m.Fecha <= fechaFin)
                .OrderBy(m => m.Fecha)
                .ToListAsync();
        }
    }
}
