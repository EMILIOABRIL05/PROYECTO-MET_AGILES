namespace Api.Ventas.Services.EventBus
{
    /// <summary>
    /// Evento que se publica cuando se confirma una venta.
    /// Preparado para integración con RabbitMQ.
    /// </summary>
    public class VentaRealizadaEvent
    {
        public int VentaId { get; set; }
        public string NumeroComprobante { get; set; } = string.Empty;
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = string.Empty;
        public decimal Total { get; set; }
        public DateTime Fecha { get; set; }
        public int CantidadProductos { get; set; }
    }
}
