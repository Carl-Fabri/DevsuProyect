using DB;
using DB.Models;
using DevsuBackend.Services.Interfaces;
using DevsuBackend.Utilities;
using Microsoft.EntityFrameworkCore;
using System;

namespace DevsuBackend.Services
{
    public class ClienteService : IClienteService
    {
        private readonly DevsuContext _context;
        private readonly ILogger<ClienteService> _logger;
        private readonly IEncryptionHelper _encryptionHelper;

        public ClienteService(DevsuContext context, ILogger<ClienteService> logger, IEncryptionHelper encryptionHelper)
        {
            _context = context;
            _logger = logger;
            _encryptionHelper = encryptionHelper;
        }

        public async Task<IEnumerable<Cliente>> GetAllClientesAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<Cliente> CreateClienteAsync(Cliente cliente)
        {
            // Encriptar contraseña antes de guardar
            cliente.Contrasena = _encryptionHelper.HashPassword(cliente.Contrasena);

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<Cliente> UpdateClienteAsync(int id, Cliente cliente)
        {
            var existingCliente = await _context.Clientes.FindAsync(id);
            if (existingCliente == null)
                return null;

            // Actualizar propiedades
            existingCliente.Nombre = cliente.Nombre;
            existingCliente.Genero = cliente.Genero;
            existingCliente.Edad = cliente.Edad;
            existingCliente.Identificacion = cliente.Identificacion;
            existingCliente.Direccion = cliente.Direccion;
            existingCliente.Telefono = cliente.Telefono;
            existingCliente.Estado = cliente.Estado;

            // Solo actualizar contraseña si se proporcionó una nueva
            if (!string.IsNullOrEmpty(cliente.Contrasena))
            {
                existingCliente.Contrasena = _encryptionHelper.HashPassword(cliente.Contrasena);
            }

            await _context.SaveChangesAsync();
            return existingCliente;
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClienteExistsAsync(int id)
        {
            return await _context.Clientes.AnyAsync(e => e.ClienteId == id);
        }
    }
}
