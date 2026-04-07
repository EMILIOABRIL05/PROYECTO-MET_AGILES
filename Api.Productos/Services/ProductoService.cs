using Api.Productos.Data;
using Api.Productos.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Productos.Services
{
    public interface IProductoService
    {
        Task<List<Producto>> ObtenerTodosAsync();
        Task<Producto?> ObtenerPorIdAsync(int id);
        Task<Producto?> ObtenerPorCodigoAsync(string codigo);
        Task<List<Producto>> BuscarPorNombreAsync(string nombre);
        Task<Producto> CrearAsync(Producto producto);
        Task<Producto?> ActualizarAsync(int id, Producto producto);
        Task<bool> ActualizarStockAsync(int id, int cantidad);
        Task<bool> EliminarAsync(int id);
    }

    public class ProductoService : IProductoService
    {
        private readonly ProductosDbContext _context;

        public ProductoService(ProductosDbContext context)
        {
            _context = context;
        }

        public async Task<List<Producto>> ObtenerTodosAsync()
        {
            return await _context.Productos.Where(p => p.Activo).ToListAsync();
        }

        public async Task<Producto?> ObtenerPorIdAsync(int id)
        {
            return await _context.Productos.FindAsync(id);
        }

        public async Task<Producto?> ObtenerPorCodigoAsync(string codigo)
        {
            return await _context.Productos.FirstOrDefaultAsync(p => p.Codigo == codigo);
        }

        public async Task<List<Producto>> BuscarPorNombreAsync(string nombre)
        {
            return await _context.Productos
                .Where(p => p.Nombre.Contains(nombre) && p.Activo)
                .ToListAsync();
        }

        public async Task<Producto> CrearAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return producto;
        }

        public async Task<Producto?> ActualizarAsync(int id, Producto producto)
        {
            var existente = await _context.Productos.FindAsync(id);
            if (existente == null) return null;

            existente.Codigo = producto.Codigo;
            existente.Nombre = producto.Nombre;
            existente.Descripcion = producto.Descripcion;
            existente.Precio = producto.Precio;
            existente.Stock = producto.Stock;
            existente.Categoria = producto.Categoria;
            existente.Activo = producto.Activo;

            await _context.SaveChangesAsync();
            return existente;
        }

        public async Task<bool> ActualizarStockAsync(int id, int cantidad)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null || producto.Stock < cantidad) return false;

            producto.Stock -= cantidad;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null) return false;

            producto.Activo = false; // Soft delete
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
