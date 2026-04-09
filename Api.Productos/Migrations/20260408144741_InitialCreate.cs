using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Api.Productos.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Productos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Codigo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Precio = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Stock = table.Column<int>(type: "int", nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Productos", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Productos",
                columns: new[] { "Id", "Activo", "Categoria", "Codigo", "Descripcion", "Nombre", "Precio", "Stock" },
                values: new object[,]
                {
                    { 1, true, "Computadoras", "PROD-001", "Laptop de alto rendimiento con pantalla OLED 4K", "Laptop Dell XPS 15", 1899.99m, 15 },
                    { 2, true, "Periféricos", "PROD-002", "Mouse inalámbrico ergonómico de alta precisión", "Mouse Logitech MX Master 3", 99.99m, 50 },
                    { 3, true, "Periféricos", "PROD-003", "Teclado mecánico inalámbrico con switches Brown", "Teclado Mecánico Keychron K2", 89.99m, 35 },
                    { 4, true, "Monitores", "PROD-004", "Monitor 4K UHD de 27 pulgadas con USB-C", "Monitor LG 27UL850", 449.99m, 20 },
                    { 5, true, "Periféricos", "PROD-005", "Cámara web Full HD 1080p con micrófono estéreo", "Webcam Logitech C920", 79.99m, 60 },
                    { 6, true, "Audio", "PROD-006", "Audífonos over-ear con cancelación de ruido activa", "Audífonos Sony WH-1000XM5", 349.99m, 30 },
                    { 7, true, "Accesorios", "PROD-007", "Cable HDMI 2.1 de 2 metros, soporta 8K@60Hz", "Cable HDMI 2.1 Ultra High Speed", 19.99m, 100 },
                    { 8, true, "Almacenamiento", "PROD-008", "Unidad de estado sólido NVMe PCIe 4.0, lectura 7450 MB/s", "SSD Samsung 990 Pro 1TB", 159.99m, 40 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Productos_Codigo",
                table: "Productos",
                column: "Codigo",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Productos");
        }
    }
}
