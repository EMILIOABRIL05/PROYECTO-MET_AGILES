using Api.Clientes.Data;
using Api.Clientes.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Clientes.Services
{
    public interface IClienteService
    {
        Task<List<Cliente>> ObtenerTodosAsync();
        Task<Cliente?> ObtenerPorIdAsync(int id);
        Task<Cliente?> ObtenerPorCedulaAsync(string cedula);
        Task<Cliente> CrearAsync(Cliente cliente);
        Task<Cliente?> ActualizarAsync(int id, Cliente cliente);
        Task<bool> EliminarAsync(int id);
    }

    public class ClienteService : IClienteService
    {
        private readonly ClientesDbContext _context;

        public ClienteService(ClientesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            return await _context.Clientes.ToListAsync();
        }

        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            return await _context.Clientes.FindAsync(id);
        }

        public async Task<Cliente?> ObtenerPorCedulaAsync(string cedula)
        {
            return await _context.Clientes.FirstOrDefaultAsync(c => c.Cedula == cedula);
        }

        public async Task<Cliente> CrearAsync(Cliente cliente)
        {
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return cliente;
        }

        public async Task<Cliente?> ActualizarAsync(int id, Cliente cliente)
        {
            var existente = await _context.Clientes.FindAsync(id);
            if (existente == null) return null;

            existente.Cedula = cliente.Cedula;
            existente.Nombres = cliente.Nombres;
            existente.Apellidos = cliente.Apellidos;
            existente.Direccion = cliente.Direccion;
            existente.Telefono = cliente.Telefono;
            existente.Email = cliente.Email;

            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return false;

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
