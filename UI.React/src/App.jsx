import { useState } from 'react';
import BuscadorCliente from './components/BuscadorCliente';
import ModalProductos from './components/ModalProductos';
import { ventasApi, clientesApi } from './services/api';
import { Receipt, FilePlus, Plus, ClipboardList, Trash2, CheckCircle, Loader2 } from 'lucide-react';
import carritoIcon from './assets/carrito.svg';
import './App.css';

const IVA_PORCENTAJE = 0.15; // 15% IVA

function App() {

  // Agrega este estado junto a los demás:
const [resetKey, setResetKey] = useState(0);

  // Estado del cliente seleccionado
  const [cliente, setCliente] = useState(null);

  // Estado del número de comprobante
  const [comprobante, setComprobante] = useState('Pendiente...');

  // Estado del detalle de venta
  const [detalles, setDetalles] = useState([]);

  // Modal de productos
  const [modalAbierto, setModalAbierto] = useState(false);

  // Producto temporal antes de ir al grid
  const [productoTemporal, setProductoTemporal] = useState(null);
  const [cantidadTemporal, setCantidadTemporal] = useState(1);

  // Estado de envío
  const [enviando, setEnviando] = useState(false);
  const [mensaje, setMensaje] = useState({ tipo: '', texto: '' });

  // ═══ Cálculos de IVA y Total ═══════════════════════════
  const calcularSubtotalSinIVA = () => {
    return detalles.reduce((sum, d) => sum + d.subtotal, 0);
  };

  const calcularIVA = () => {
    return calcularSubtotalSinIVA() * IVA_PORCENTAJE;
  };

  const calcularTotal = () => {
    return calcularSubtotalSinIVA() + calcularIVA();
  };

  // ═══ Manejo del detalle ════════════════════════════════
  const seleccionarParaAgregar = (detalle) => {
    setProductoTemporal(detalle);
    setCantidadTemporal(detalle.cantidad || 1);
    setModalAbierto(false);
  };

  const agregarAlGrid = () => {
    if (!productoTemporal) return;

    const cant = parseInt(cantidadTemporal) || 1;
    const desc = productoTemporal;

    const existente = detalles.findIndex(d => d.productoId === desc.productoId);

    // Validar con el stock disponible
    let cantidadTotal = cant;
    if (existente >= 0) {
      cantidadTotal += detalles[existente].cantidad;
    }

    if (desc.stock !== undefined && cantidadTotal > desc.stock) {
      setMensaje({ tipo: 'error', texto: `⚠️ Stock insuficiente. Solo hay ${desc.stock} unidades disponibles de este producto.` });
      return;
    }

    if (existente >= 0) {
      const nuevosDetalles = [...detalles];
      nuevosDetalles[existente].cantidad += cant;
      nuevosDetalles[existente].subtotal = nuevosDetalles[existente].cantidad * nuevosDetalles[existente].precioUnitario;
      setDetalles(nuevosDetalles);
    } else {
      setDetalles(prev => [...prev, {
        ...desc,
        cantidad: cant,
        subtotal: cant * desc.precioUnitario
      }]);
    }

    setProductoTemporal(null);
    setCantidadTemporal(1);
  };

  const eliminarDetalle = (index) => {
    setDetalles(prev => prev.filter((_, i) => i !== index));
  };

  // ═══ Enviar Venta al microservicio ═════════════════════
  const confirmarVenta = async () => {
    if (!cliente) {
      setMensaje({ tipo: 'error', texto: '⚠️ Debe seleccionar un cliente' });
      return;
    }
    if (detalles.length === 0) {
      setMensaje({ tipo: 'error', texto: '⚠️ Debe agregar al menos un producto' });
      return;
    }

    setEnviando(true);
    setMensaje({ tipo: '', texto: '' });

    // Si el cliente es nuevo, lo creamos primero en la BD de Clientes
    let clienteIdFinal = cliente.id;
    let clienteFullName = `${cliente.nombres} ${cliente.apellidos}`;

    if (cliente.esNuevo) {
      if (!cliente.nombres || !cliente.apellidos || !cliente.cedula) {
        setMensaje({ tipo: 'error', texto: '⚠️ Complete los nombres, apellidos y cédula del nuevo cliente.' });
        setEnviando(false);
        return;
      }
      try {
        setMensaje({ tipo: '', texto: '⏳ Registrando nuevo cliente...' });
        const nuevoClientePayload = {
          cedula: cliente.cedula,
          nombres: cliente.nombres,
          apellidos: cliente.apellidos,
          direccion: cliente.direccion || '',
          telefono: cliente.telefono || '',
          email: cliente.email || ''
        };
        const nuevoClienteGuardado = await clientesApi.crear(nuevoClientePayload);
        clienteIdFinal = nuevoClienteGuardado.id;
        clienteFullName = `${nuevoClienteGuardado.nombres} ${nuevoClienteGuardado.apellidos}`;
      } catch (err) {
        setMensaje({ tipo: 'error', texto: `❌ Error al crear cliente: ${err.message}` });
        setEnviando(false);
        return;
      }
    }

    // Construir JSON para el microservicio de Ventas
    const ventaPayload = {
      clienteId: clienteIdFinal,
      clienteCedula: cliente.cedula,
      clienteNombre: clienteFullName,
      subtotal: parseFloat(calcularSubtotalSinIVA().toFixed(2)),
      iva: parseFloat(calcularIVA().toFixed(2)),
      total: parseFloat(calcularTotal().toFixed(2)),
      detalles: detalles.map(d => ({
        productoId: d.productoId,
        productoNombre: d.productoNombre,
        cantidad: d.cantidad,
        precioUnitario: d.precioUnitario,
        subtotal: parseFloat(d.subtotal.toFixed(2))
      }))
    };

    try {
      setMensaje({ tipo: '', texto: '⏳ Registrando venta...' });
      const resultado = await ventasApi.crear(ventaPayload);
      setComprobante(resultado.numeroComprobante);
      setMensaje({
        tipo: 'success',
        texto: `✅ Venta registrada exitosamente — Comprobante: ${resultado.numeroComprobante}`
      });
      // Limpiar formulario
      setDetalles([]);
    } catch (err) {
      setMensaje({
        tipo: 'error',
        texto: `❌ Error: ${err.message}`
      });
    } finally {
      setEnviando(false);
    }
  };

  const nuevaFactura = () => {
    setCliente(null);
    setDetalles([]);
    setMensaje({ tipo: '', texto: '' });
    setComprobante('Pendiente...');
    setResetKey(prev => prev + 1); // Cambia la clave para resetear el buscador de cliente
  };

  return (
    <div className="app-container">
      {/* Header */}
      <header className="app-header">
        <div className="header-content">
          <div className="logo-section">
            <Receipt className="logo-icon lucide-logo" size={32} color="#818cf8" />
            <div>
              <h1>VENTA DE PRODUCTOS</h1>
              <div className="header-line"></div>
            </div>
          </div>
          <button className="btn-nueva-factura" onClick={nuevaFactura}>
            <FilePlus size={18} /> Nueva Factura
          </button>
        </div>
      </header>

      <main className="main-content">
        {/* DATOS DE VENTA */}
        <div className="section-container">
          <div className="section-group-label">DATOS DE VENTA</div>
          <div className="invoice-info-bar">
            <div className="info-item">
              <span className="info-label">Fecha Venta:</span>
              <div className="info-field-box">
                <span className="info-value">{new Date().toLocaleDateString()}</span>
              </div>
            </div>
            <div className="info-item">
              <span className="info-label">N° Comprobante:</span>
              <span className="info-value-highlight">{comprobante}</span>
            </div>
          </div>
        </div>

        {/* DATOS DEL CLIENTE — Integrado en BuscadorCliente */}
        <div className="row-top">
          <BuscadorCliente onClienteSeleccionado={setCliente} resetKey={resetKey} />
        </div>

        {/* Sección Inferior — DATOS DEL DETALLE DE VENTA */}
        <div className="row-bottom">
          <div className="section-container">
            <div className="section-group-label">DATOS DEL DETALLE DE VENTA</div>
            <div className="detalle-venta">
              <div className="detalle-header">
                <h3 className="section-title">
                   Detalle de Venta
                </h3>
                <button
                  id="btn-abrir-modal-productos"
                  className="btn-agregar-producto"
                  onClick={() => setModalAbierto(true)}
                >
                  <Plus size={18} /> Producto
                </button>
              </div>

              {/* Fila intermedia — preview producto + cantidad antes de agregar al carrito */}
              <div className="preview-row">
                <div className="preview-field">
                  <label>Nombre Comercial</label>
                  <input type="text" value={productoTemporal?.productoNombre || ''} readOnly placeholder="Seleccione un producto..." />
                </div>
                <div className="preview-field preview-field--sm">
                  <label>Presentación</label>
                  <input type="text" value={productoTemporal ? 'UNIDAD' : ''} readOnly placeholder="—" />
                </div>
                <div className="preview-field preview-field--sm">
                  <label>Precio Unit.</label>
                  <input type="text" value={productoTemporal ? `$${productoTemporal.precioUnitario.toFixed(2)}` : ''} readOnly placeholder="$0.00" />
                </div>
                <div className="preview-field preview-field--xs">
                  <label>Cantidad</label>
                  <input
                    type="number"
                    min="1"
                    max={productoTemporal?.stock || ""}
                    value={cantidadTemporal}
                    onChange={(e) => setCantidadTemporal(e.target.value)}
                    disabled={!productoTemporal}
                    className="input-cantidad"
                  />
                </div>
                <button
                  className="btn-add-grid"
                  onClick={agregarAlGrid}
                  disabled={!productoTemporal}
                  title="Agregar al detalle"
                >
                  <Plus size={20} />
                </button>
              </div>

            {detalles.length === 0 ? (
              <div className="empty-detalle">
                <ClipboardList className="empty-icon" size={64} color="#64748b" strokeWidth={1} />
                <p>No hay productos en el carrito</p>
                <p className="empty-hint">Use el botón "Producto" para buscar y luego agregue con "+"</p>
              </div>
            ) : (
              <div className="detalle-table-wrapper">
                <table className="detalle-table">
                  <thead>
                    <tr>
                      <th>Id</th>
                      <th>NombreComercial</th>
                      <th>Presentacion</th>
                      <th>Cantidad</th>
                      <th>PrecioVenta</th>
                      <th>Subtotal</th>
                      <th></th>
                    </tr>
                  </thead>
                  <tbody>
                    {detalles.map((d, i) => (
                      <tr key={i}>
                        <td className="td-num">{i + 1}</td>
                        <td className="td-producto">{d.productoNombre}</td>
                        <td>UNIDAD</td>
                        <td className="td-cant">{d.cantidad}</td>
                        <td className="td-precio">${d.precioUnitario.toFixed(2)}</td>
                        <td className="td-subtotal">${d.subtotal.toFixed(2)}</td>
                        <td className="td-eliminar">
                          <button
                            className="btn-eliminar-item"
                            onClick={() => eliminarDetalle(i)}
                            title="Eliminar"
                          >
                            <Trash2 size={18} color="#ef4444" />
                          </button>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            )}

            {/* Totales */}
            <div className="totales-section">
              <div className="total-row">
                <span>Subtotal:</span>
                <span className="total-value">${calcularSubtotalSinIVA().toFixed(2)}</span>
              </div>
              <div className="total-row">
                <span>IVA (15%):</span>
                <span className="total-value iva">${calcularIVA().toFixed(2)}</span>
              </div>
              <div className="total-row total-final">
                <span>TOTAL:</span>
                <span className="total-value final">${calcularTotal().toFixed(2)}</span>
              </div>
            </div>

            {/* Mensaje */}
            {mensaje.texto && (
              <div className={`mensaje ${mensaje.tipo}`}>
                {mensaje.texto}
              </div>
            )}

            {/* Botón Confirmar */}
            <button
              id="btn-confirmar-venta"
              className="btn-confirmar"
              onClick={confirmarVenta}
              disabled={enviando}
            >
              {enviando ? (
                <>
                  <Loader2 className="spinner-btn lucide-spin" size={18} /> Procesando...
                </>
              ) : (
                <>
                  <CheckCircle size={20} /> Confirmar Venta
                </>
              )}
            </button>
          </div>
        </div>
      </div>
    </main>

      {/* Modal de Productos */}
      <ModalProductos
        isOpen={modalAbierto}
        onClose={() => setModalAbierto(false)}
        onProductoSeleccionado={seleccionarParaAgregar}
      />
    </div>
  );
}

export default App;
