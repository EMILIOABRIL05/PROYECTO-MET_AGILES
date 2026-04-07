import { useState, useEffect } from 'react';
import { productosApi } from '../services/api';
import { Package, X, Loader2, PlusCircle } from 'lucide-react';
import './ModalProductos.css';

/**
 * ModalProductos — Ventana emergente que consume Api.Productos
 * y permite inyectar el producto seleccionado al detalle de venta.
 */
export default function ModalProductos({ isOpen, onClose, onProductoSeleccionado }) {
  const [productos, setProductos] = useState([]);
  const [filtro, setFiltro] = useState('');
  const [cargando, setCargando] = useState(false);
  const [cantidad, setCantidad] = useState({});

  useEffect(() => {
    if (isOpen) {
      cargarProductos();
    }
  }, [isOpen]);

  const cargarProductos = async () => {
    setCargando(true);
    try {
      const data = await productosApi.obtenerTodos();
      setProductos(data);
    } catch (err) {
      console.error('Error cargando productos:', err);
    } finally {
      setCargando(false);
    }
  };

  const productosFiltrados = productos.filter(p =>
    p.nombre.toLowerCase().includes(filtro.toLowerCase()) ||
    p.codigo.toLowerCase().includes(filtro.toLowerCase()) ||
    p.categoria.toLowerCase().includes(filtro.toLowerCase())
  );

  const seleccionarProducto = (producto) => {
    const cant = cantidad[producto.id] || 1;
    if (cant > producto.stock) {
      alert(`Stock insuficiente. Disponible: ${producto.stock}`);
      return;
    }

    const detalle = {
      productoId: producto.id,
      productoNombre: producto.nombre,
      cantidad: cant,
      precioUnitario: producto.precio,
      subtotal: cant * producto.precio
    };

    onProductoSeleccionado(detalle);
    setCantidad(prev => ({ ...prev, [producto.id]: 1 }));
  };

  const handleCantidadChange = (id, value) => {
    const val = parseInt(value) || 1;
    setCantidad(prev => ({ ...prev, [id]: Math.max(1, val) }));
  };

  if (!isOpen) return null;

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h3>
            <Package className="icon" size={24} /> Seleccionar Producto
          </h3>
          <button className="modal-close" onClick={onClose}><X size={18} /></button>
        </div>

        <div className="modal-search">
          <input
            id="input-buscar-producto"
            type="text"
            placeholder="Buscar por nombre, código o categoría..."
            value={filtro}
            onChange={(e) => setFiltro(e.target.value)}
            className="input-filtro"
            autoFocus
          />
        </div>

        <div className="modal-body">
          {cargando ? (
            <div className="loading-state">
              <Loader2 className="lucide-spin" size={40} color="#818cf8" />
              <p>Cargando productos...</p>
            </div>
          ) : productosFiltrados.length === 0 ? (
            <div className="empty-state">
              <p>No se encontraron productos</p>
            </div>
          ) : (
            <table className="productos-table">
              <thead>
                <tr>
                  <th>ID</th>
                  <th>Código</th>
                  <th>Nombre</th>
                  <th>Stock</th>
                  <th>Precio</th>
                  <th>Cantidad</th>
                  <th>Acción</th>
                </tr>
              </thead>
              <tbody>
                {productosFiltrados.map(p => (
                  <tr key={p.id} className={p.stock === 0 ? 'sin-stock' : ''}>
                    <td className="td-id">{p.id}</td>
                    <td className="td-codigo">{p.codigo}</td>
                    <td className="td-nombre">{p.nombre}</td>
                    <td className={`td-stock ${p.stock <= 5 ? 'stock-bajo' : ''}`}>
                      {p.stock}
                    </td>
                    <td className="td-precio">${p.precio.toFixed(2)}</td>
                    <td className="td-cantidad">
                      <input
                        type="number"
                        min="1"
                        max={p.stock}
                        value={cantidad[p.id] || 1}
                        onChange={(e) => handleCantidadChange(p.id, e.target.value)}
                        className="input-cantidad"
                        disabled={p.stock === 0}
                      />
                    </td>
                    <td className="td-accion">
                      <button
                        className="btn-agregar"
                        onClick={() => seleccionarProducto(p)}
                        disabled={p.stock === 0}
                        title={p.stock === 0 ? 'Sin stock' : 'Agregar al detalle'}
                      >
                        <PlusCircle size={20} />
                      </button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>
      </div>
    </div>
  );
}
