using DB.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DevsuBackend.Services.Interfaces
{
    public interface ICuentaService
    {
        Task<IEnumerable<Cuenta>> GetAllCuentasAsync();
        Task<Cuenta> GetCuentaByIdAsync(int id);
        Task<Cuenta> GetCuentaByNumeroAsync(int numeroCuenta);
        Task<Cuenta> CreateCuentaAsync(Cuenta cuenta);
        Task<Cuenta> UpdateCuentaAsync(int id, Cuenta cuenta);
        Task<bool> DeleteCuentaAsync(int id);
        Task<bool> CuentaExistsAsync(int id);
        Task<decimal> GetSaldoDisponibleAsync(int cuentaId);
    }
}
