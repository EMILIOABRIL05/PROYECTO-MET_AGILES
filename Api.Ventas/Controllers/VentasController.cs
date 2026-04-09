using Api.Ventas.Models;
using Api.Ventas.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Ventas.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IVentaService _service;

        public VentasController(IVentaService service)
        {
            _service = service;
        }

        // GET: api/ventas
        [HttpGet]
        public async Task<ActionResult<List<Venta>>> ObtenerTodos()
        {
            var ventas = await _service.ObtenerTodosAsync();
            return Ok(ventas);
        }

        // GET: api/ventas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Venta>> ObtenerPorId(int id)
        {
            var venta = await _service.ObtenerPorIdAsync(id);
            if (venta == null) return NotFound();
            return Ok(venta);
        }

        // GET: api/ventas/cliente/3
        [HttpGet("cliente/{clienteId}")]
        public async Task<ActionResult<List<Venta>>> ObtenerPorCliente(int clienteId)
        {
            var ventas = await _service.ObtenerPorClienteAsync(clienteId);
            return Ok(ventas);
        }

        // POST: api/ventas
        [HttpPost]
        public async Task<ActionResult<Venta>> Crear([FromBody] Venta venta)
        {
            if (venta.Detalles == null || venta.Detalles.Count == 0)
                return BadRequest("La venta debe contener al menos un detalle.");

            // Generar NumeroComprobante si viene vacío
            if (string.IsNullOrEmpty(venta.NumeroComprobante))
                venta.NumeroComprobante = $"{DateTime.Now.Year}-UTA-{new Random().Next(1000, 9999)}";

            // Asignar VentaId a cada detalle y limpiar navegación
            foreach (var detalle in venta.Detalles)
            {
                detalle.VentaId = 0; // EF lo asigna solo al insertar
                detalle.Venta = null!;
            }

            var creada = await _service.CrearAsync(venta);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, creada);
        }
    }
}
