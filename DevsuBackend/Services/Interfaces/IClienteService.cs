using DB.Models;

namespace DevsuBackend.Services.Interfaces
{
    public interface IClienteService
    {
        Task<IEnumerable<Cliente>> GetAllClientesAsync();
        Task<Cliente> GetClienteByIdAsync(int id);
        Task<Cliente> CreateClienteAsync(Cliente cliente);
        Task<Cliente> UpdateClienteAsync(int id, Cliente cliente);
        Task<bool> DeleteClienteAsync(int id);
        Task<bool> ClienteExistsAsync(int id);
    }
}
