using Api.Ventas.Data;
using Api.Ventas.Models;
using Api.Ventas.Services.EventBus;
using Microsoft.EntityFrameworkCore;

namespace Api.Ventas.Services
{
    public interface IVentaService
    {
        Task<List<Venta>> ObtenerTodosAsync();
        Task<Venta?> ObtenerPorIdAsync(int id);
        Task<Venta> CrearAsync(Venta venta);
        Task<List<Venta>> ObtenerPorClienteAsync(int clienteId);
    }

    public class VentaService : IVentaService
    {
        private readonly VentasDbContext _context;
        private readonly IEventBus _eventBus;
        private readonly IHttpClientFactory _httpClientFactory;

        public VentaService(VentasDbContext context, IEventBus eventBus, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _eventBus = eventBus;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<List<Venta>> ObtenerTodosAsync()
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }

        public async Task<Venta?> ObtenerPorIdAsync(int id)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Venta> CrearAsync(Venta venta)
        {
            // Generar número de comprobante automático
            var ultimaVenta = await _context.Ventas.OrderByDescending(v => v.Id).FirstOrDefaultAsync();
            var secuencia = (ultimaVenta?.Id ?? 0) + 1;
            venta.NumeroComprobante = $"FAC-{DateTime.Now:yyyyMMdd}-{secuencia:D5}";
            venta.Fecha = DateTime.Now;

            _context.Ventas.Add(venta);
            await _context.SaveChangesAsync();

            var httpClient = _httpClientFactory.CreateClient();
            foreach (var detalle in venta.Detalles)
            {
                try
                {
                    await httpClient.PatchAsync(
                    $"http://localhost:5020/api/productos/{detalle.ProductoId}/stock?cantidad={detalle.Cantidad}",
                    null
                    );
                }
                catch
                {
                    // Si falla el descuento de stock, la venta igual se guarda
                }
            }

            // Publicar evento VentaRealizada al Event Bus (RabbitMQ)
            var evento = new VentaRealizadaEvent
            {
                VentaId = venta.Id,
                NumeroComprobante = venta.NumeroComprobante,
                ClienteId = venta.ClienteId,
                ClienteNombre = venta.ClienteNombre,
                Total = venta.Total,
                Fecha = venta.Fecha,
                CantidadProductos = venta.Detalles.Count
            };
            await _eventBus.PublicarVentaRealizadaAsync(evento);

            return venta;
        }

        public async Task<List<Venta>> ObtenerPorClienteAsync(int clienteId)
        {
            return await _context.Ventas
                .Include(v => v.Detalles)
                .Where(v => v.ClienteId == clienteId)
                .OrderByDescending(v => v.Fecha)
                .ToListAsync();
        }
    }
}
