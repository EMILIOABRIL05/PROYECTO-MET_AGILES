using Api.Clientes.Models;
using Api.Clientes.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Clientes.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        private readonly IClienteService _service;

        public ClientesController(IClienteService service)
        {
            _service = service;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> ObtenerTodos()
        {
            var clientes = await _service.ObtenerTodosAsync();
            return Ok(clientes);
        }

        // GET: api/clientes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Cliente>> ObtenerPorId(int id)
        {
            var cliente = await _service.ObtenerPorIdAsync(id);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // GET: api/clientes/cedula/0501234567
        [HttpGet("cedula/{cedula}")]
        public async Task<ActionResult<Cliente>> ObtenerPorCedula(string cedula)
        {
            var cliente = await _service.ObtenerPorCedulaAsync(cedula);
            if (cliente == null) return NotFound();
            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> Crear([FromBody] Cliente cliente)
        {
            var creado = await _service.CrearAsync(cliente);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
        }

        // PUT: api/clientes/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Cliente>> Actualizar(int id, [FromBody] Cliente cliente)
        {
            var actualizado = await _service.ActualizarAsync(id, cliente);
            if (actualizado == null) return NotFound();
            return Ok(actualizado);
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.EliminarAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }
    }
}
