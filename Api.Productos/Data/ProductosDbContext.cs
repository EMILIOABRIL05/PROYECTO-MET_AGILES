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

            modelBuilder.Entity<Producto>().HasData(
                new Producto
                {
                    Id = 1,
                    Codigo = "PROD-001",
                    Nombre = "Laptop Dell XPS 15",
                    Descripcion = "Laptop de alto rendimiento con pantalla OLED 4K",
                    Precio = 1899.99m,
                    Stock = 15,
                    Categoria = "Computadoras",
                    Activo = true
                },
                new Producto
                {
                    Id = 2,
                    Codigo = "PROD-002",
                    Nombre = "Mouse Logitech MX Master 3",
                    Descripcion = "Mouse inalámbrico ergonómico de alta precisión",
                    Precio = 99.99m,
                    Stock = 50,
                    Categoria = "Periféricos",
                    Activo = true
                },
                new Producto
                {
                    Id = 3,
                    Codigo = "PROD-003",
                    Nombre = "Teclado Mecánico Keychron K2",
                    Descripcion = "Teclado mecánico inalámbrico con switches Brown",
                    Precio = 89.99m,
                    Stock = 35,
                    Categoria = "Periféricos",
                    Activo = true
                },
                new Producto
                {
                    Id = 4,
                    Codigo = "PROD-004",
                    Nombre = "Monitor LG 27UL850",
                    Descripcion = "Monitor 4K UHD de 27 pulgadas con USB-C",
                    Precio = 449.99m,
                    Stock = 20,
                    Categoria = "Monitores",
                    Activo = true
                },
                new Producto
                {
                    Id = 5,
                    Codigo = "PROD-005",
                    Nombre = "Webcam Logitech C920",
                    Descripcion = "Cámara web Full HD 1080p con micrófono estéreo",
                    Precio = 79.99m,
                    Stock = 60,
                    Categoria = "Periféricos",
                    Activo = true
                },
                new Producto
                {
                    Id = 6,
                    Codigo = "PROD-006",
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