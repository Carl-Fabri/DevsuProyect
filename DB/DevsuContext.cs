using DB.Models;
using DB.SP;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DB
{
    public class DevsuContext : DbContext
    {
        public DevsuContext(DbContextOptions<DevsuContext> options): base(options)
        {
        }

        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Cuenta> Cuentas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Persona>()
                .HasDiscriminator<string>("PersonaType")
                .HasValue<Persona>("Persona")
                .HasValue<Cliente>("Cliente");

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.Identificacion).IsUnique();
            });

            modelBuilder.Entity<Cuenta>(entity =>
            {
                entity.HasIndex(c => c.NumeroCuenta).IsUnique();
                entity.HasCheckConstraint("CK_TipoCuenta", "TipoCuenta IN ('Ahorro', 'Corriente')");
                entity.HasOne(c => c.Cliente)
                      .WithMany()
                      .HasForeignKey(c => c.ClienteId);
            });

            modelBuilder.Entity<Movimiento>(entity =>
            {
                entity.HasCheckConstraint("CK_TipoMovimiento", "TipoMovimiento IN ('Deposito', 'Retiro')");
                entity.HasOne(m => m.Cuenta)
                      .WithMany(c => c.Movimientos)
                      .HasForeignKey(m => m.CuentaId);
            });
        }
    }
}
