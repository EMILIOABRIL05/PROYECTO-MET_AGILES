using Microsoft.EntityFrameworkCore;
using Api.Clientes.Models;

namespace Api.Clientes.Data
{
    public class ClientesDbContext : DbContext
    {
        public ClientesDbContext(DbContextOptions<ClientesDbContext> options) : base(options) { }

        public DbSet<Cliente> Clientes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasIndex(c => c.Cedula).IsUnique();
                entity.Property(c => c.Cedula).IsRequired();
                entity.Property(c => c.Nombres).IsRequired();
                entity.Property(c => c.Apellidos).IsRequired();
            });
        }
    }
}
