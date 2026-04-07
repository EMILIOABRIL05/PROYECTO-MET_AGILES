using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Api.Ventas.Models
{
    /// <summary>
    /// Entidad Detalle — Cada línea de producto dentro de una venta.
    /// REGLA ORO: Contiene propiedades de navegación completas hacia Venta y Producto.
    /// EF Core las maneja como Foreign Keys conceptuales.
    /// </summary>
    public class VentaDetalle
    {
        [Key]
        public int Id { get; set; }

        // ═══════════════════════════════════════════════════════════
        // FOREIGN KEY → Venta (Propiedad de Navegación completa)
        // ═══════════════════════════════════════════════════════════
        [Required]
        public int VentaId { get; set; }

        [JsonIgnore]
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = null!;

        // ═══════════════════════════════════════════════════════════
        // FOREIGN KEY CONCEPTUAL → Producto (instanciación de clase)
        // Referencia al microservicio de Productos
        // ═══════════════════════════════════════════════════════════
        [Required]
        public int ProductoId { get; set; }

        [StringLength(200)]
        public string ProductoNombre { get; set; } = string.Empty;

        [Required]
        public int Cantidad { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }
    }
}
