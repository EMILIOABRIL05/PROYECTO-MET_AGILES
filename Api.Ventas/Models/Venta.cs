using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Ventas.Models
{
    /// <summary>
    /// Entidad Maestro — Representa la cabecera de una venta/factura.
    /// </summary>
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string NumeroComprobante { get; set; } = string.Empty;

        // Datos del cliente (referencia conceptual al microservicio de Clientes)
        public int ClienteId { get; set; }

        [StringLength(13)]
        public string ClienteCedula { get; set; } = string.Empty;

        [StringLength(200)]
        public string ClienteNombre { get; set; } = string.Empty;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Subtotal { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal IVA { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        // REGLA ORO: Lista de VentaDetalle — Relación Maestro-Detalle
        public List<VentaDetalle> Detalles { get; set; } = new List<VentaDetalle>();
    }
}
