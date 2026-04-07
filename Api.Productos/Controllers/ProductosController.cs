using Api.Productos.Models;
using Api.Productos.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Productos.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoService _service;

        public ProductosController(IProductoService service)
        {
            _service = service;
        }

        // GET: api/productos
        [HttpGet]
        public async Task<ActionResult<List<Producto>>> ObtenerTodos()
        {
            var productos = await _service.ObtenerTodosAsync();
            return Ok(productos);
        }

        // GET: api/productos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> ObtenerPorId(int id)
        {
            var producto = await _service.ObtenerPorIdAsync(id);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        // GET: api/productos/codigo/PROD-001
        [HttpGet("codigo/{codigo}")]
        public async Task<ActionResult<Producto>> ObtenerPorCodigo(string codigo)
        {
            var producto = await _service.ObtenerPorCodigoAsync(codigo);
            if (producto == null) return NotFound();
            return Ok(producto);
        }

        // GET: api/productos/buscar?nombre=laptop
        [HttpGet("buscar")]
        public async Task<ActionResult<List<Producto>>> BuscarPorNombre([FromQuery] string nombre)
        {
            var productos = await _service.BuscarPorNombreAsync(nombre);
            return Ok(productos);
        }

        // POST: api/productos
        [HttpPost]
        public async Task<ActionResult<Producto>> Crear([FromBody] Producto producto)
        {
            var creado = await _service.CrearAsync(producto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        // PUT: api/productos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> Actualizar(int id, [FromBody] Producto producto)
        {
            var actualizado = await _service.ActualizarAsync(id, producto);
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        // PATCH: api/productos/5/stock?cantidad=3
        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> ActualizarStock(int id, [FromQuery] int cantidad)
        {
            var resultado = await _service.ActualizarStockAsync(id, cantidad);
            if (!resultado) return BadRequest("Stock insuficiente o producto no encontrado.");
            return Ok();
        }

        // DELETE: api/productos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
