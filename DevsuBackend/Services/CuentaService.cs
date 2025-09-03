using DB;
using DB.Models;
using DevsuBackend.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace DevsuBackend.Services
{
    public class CuentaService: ICuentaService
    {
        private readonly DevsuContext _context;
        private readonly ILogger<CuentaService> _logger;

        public CuentaService(DevsuContext context, ILogger<CuentaService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Cuenta>> GetAllCuentasAsync()
        {
            return await _context.Cuentas.Include(c => c.Cliente).ToListAsync();
        }

        public async Task<Cuenta> GetCuentaByIdAsync(int id)
        {
            return await _context.Cuentas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.CuentaId == id);
        }

        public async Task<Cuenta> GetCuentaByNumeroAsync(int numeroCuenta)
        {
            return await _context.Cuentas
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.NumeroCuenta == numeroCuenta);
        }

        public async Task<Cuenta> CreateCuentaAsync(Cuenta cuenta)
        {
            _context.Cuentas.Add(cuenta);
            await _context.SaveChangesAsync();
            return cuenta;
        }

        public async Task<Cuenta> UpdateCuentaAsync(int id, Cuenta cuenta)
        {
            var existingCuenta = await _context.Cuentas.FindAsync(id);
            if (existingCuenta == null)
                return null;

            existingCuenta.NumeroCuenta = cuenta.NumeroCuenta;
            existingCuenta.TipoCuenta = cuenta.TipoCuenta;
            existingCuenta.SaldoInicial = cuenta.SaldoInicial;
            existingCuenta.Estado = cuenta.Estado;
            existingCuenta.ClienteId = cuenta.ClienteId;

            await _context.SaveChangesAsync();
            return existingCuenta;
        }

        public async Task<bool> DeleteCuentaAsync(int id)
        {
            var cuenta = await _context.Cuentas.FindAsync(id);
            if (cuenta == null)
                return false;

            _context.Cuentas.Remove(cuenta);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CuentaExistsAsync(int id)
        {
            return await _context.Cuentas.AnyAsync(e => e.CuentaId == id);
        }

        public async Task<decimal> GetSaldoDisponibleAsync(int cuentaId)
        {
            var cuenta = await _context.Cuentas.FindAsync(cuentaId);
            return cuenta?.SaldoInicial ?? 0;
        }
    }
}
