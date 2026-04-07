using Microsoft.EntityFrameworkCore;
using Api.Ventas.Models;

namespace Api.Ventas.Data
{
    public class VentasDbContext : DbContext
    {
        public VentasDbContext(DbContextOptions<VentasDbContext> options) : base(options) { }

        public DbSet<Venta> Ventas { get; set; }
        public DbSet<VentaDetalle> VentaDetalles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración de Venta (Maestro)
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasIndex(v => v.NumeroComprobante).IsUnique();
                entity.Property(v => v.NumeroComprobante).IsRequired();
                entity.Property(v => v.Total).HasColumnType("decimal(18,2)");
                entity.Property(v => v.Subtotal).HasColumnType("decimal(18,2)");
                entity.Property(v => v.IVA).HasColumnType("decimal(18,2)");

                // Relación Maestro-Detalle con EF Core
                entity.HasMany(v => v.Detalles)
                      .WithOne(d => d.Venta)
                      .HasForeignKey(d => d.VentaId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configuración de VentaDetalle (Detalle)
            modelBuilder.Entity<VentaDetalle>(entity =>
            {
                entity.Property(d => d.PrecioUnitario).HasColumnType("decimal(18,2)");
                entity.Property(d => d.Subtotal).HasColumnType("decimal(18,2)");
            });
        }
    }
}
