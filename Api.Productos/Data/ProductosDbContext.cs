using Microsoft.EntityFrameworkCore;
using Api.Productos.Models;

namespace Api.Productos.Data
{
    public class ProductosDbContext : DbContext
    {
        public ProductosDbContext(DbContextOptions<ProductosDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasIndex(p => p.Codigo).IsUnique();
                entity.Property(p => p.Codigo).IsRequired();
                entity.Property(p => p.Nombre).IsRequired();
                entity.Property(p => p.Precio).HasColumnType("decimal(18,2)");
            });

                    Nombre = "Audífonos Sony WH-1000XM5",
                    Descripcion = "Audífonos over-ear con cancelación de ruido activa",
                    Precio = 349.99m,
                    Stock = 30,
                    Categoria = "Audio",
                    Activo = true
                },
                new Producto
                {
                    Id = 7,
                    Codigo = "PROD-007",
                    Nombre = "Cable HDMI 2.1 Ultra High Speed",
                    Descripcion = "Cable HDMI 2.1 de 2 metros, soporta 8K@60Hz",
                    Precio = 19.99m,
                    Stock = 100,
                    Categoria = "Accesorios",
                    Activo = true
                },
                new Producto
                {
                    Id = 8,
                    Codigo = "PROD-008",
                    Nombre = "SSD Samsung 990 Pro 1TB",
                    Descripcion = "Unidad de estado sólido NVMe PCIe 4.0, lectura 7450 MB/s",
                    Precio = 159.99m,
                    Stock = 40,
                    Categoria = "Almacenamiento",
                    Activo = true
                }
            );
        }
    }
}
