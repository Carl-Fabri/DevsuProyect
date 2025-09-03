using DB.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using BCrypt.Net;

namespace DB
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<DevsuContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

            await context.Database.MigrateAsync();

            if (!context.Clientes.Any())
            {
                // Hash helper local (salt rounds 12)
                string Hash(string plain) => BCrypt.Net.BCrypt.HashPassword(plain, BCrypt.Net.BCrypt.GenerateSalt(12));

                var clientes = new List<Cliente>
                {
                    new Cliente {ClienteId=1, Nombre = "Jose Lema", Direccion = "Otavalo sn y principal", Telefono = "098254785", Contrasena = Hash("1234"), Estado = true, Genero="M", Edad=30, Identificacion="0101", PersonaType="Cliente" },
                    new Cliente {ClienteId=2, Nombre = "Marianela Montalvo", Direccion = "Amazonas y NNUU", Telefono = "097548965", Contrasena = Hash("5678"), Estado = true, Genero="F", Edad=28, Identificacion="0202", PersonaType="Cliente" },
                    new Cliente {ClienteId=3, Nombre = "Juan Osorio", Direccion = "13 junio y Equinoccial", Telefono = "098874587", Contrasena = Hash("1245"), Estado = true, Genero="M", Edad=35, Identificacion="0303", PersonaType="Cliente" }
                };
                context.Clientes.AddRange(clientes);
                await context.SaveChangesAsync();

                var jose = clientes[0];
                var marianela = clientes[1];
                var juan = clientes[2];

                var cuentas = new List<Cuenta>
                {
                    new Cuenta { NumeroCuenta = 478758, TipoCuenta = "Ahorro", SaldoInicial = 2000, Estado = true, ClienteId = jose.ClienteId },
                    new Cuenta { NumeroCuenta = 225487, TipoCuenta = "Corriente", SaldoInicial = 100, Estado = true, ClienteId = marianela.ClienteId },
                    new Cuenta { NumeroCuenta = 495878, TipoCuenta = "Ahorro", SaldoInicial = 0, Estado = true, ClienteId = juan.ClienteId },
                    new Cuenta { NumeroCuenta = 496825, TipoCuenta = "Ahorro", SaldoInicial = 540, Estado = true, ClienteId = marianela.ClienteId },
                    new Cuenta { NumeroCuenta = 585545, TipoCuenta = "Corriente", SaldoInicial = 1000, Estado = true, ClienteId = jose.ClienteId }
                };
                context.Cuentas.AddRange(cuentas);
                await context.SaveChangesAsync();

                // Ajustar saldos tras movimientos
                var movimientos = new List<Movimiento>
                {
                    new Movimiento { CuentaId = cuentas[0].CuentaId, Fecha = DateTime.UtcNow.AddDays(-2), TipoMovimiento = "Retiro", Valor = -575, Saldo = 2000 - 575 },
                    new Movimiento { CuentaId = cuentas[1].CuentaId, Fecha = DateTime.UtcNow.AddDays(-1), TipoMovimiento = "Deposito", Valor = 600, Saldo = 100 + 600 },
                    new Movimiento { CuentaId = cuentas[2].CuentaId, Fecha = DateTime.UtcNow.AddDays(-1), TipoMovimiento = "Deposito", Valor = 150, Saldo = 0 + 150 },
                    new Movimiento { CuentaId = cuentas[3].CuentaId, Fecha = DateTime.UtcNow, TipoMovimiento = "Retiro", Valor = -540, Saldo = 540 - 540 }
                };
                context.Movimientos.AddRange(movimientos);
                await context.SaveChangesAsync();
            }
        }
    }
}
