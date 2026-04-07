// ═══════════════════════════════════════════════════════════
// API Service — Comunicación con los microservicios
// El Gateway Ocelot unifica todos los endpoints en :5000
// ═══════════════════════════════════════════════════════════

const API_BASE_URL = 'http://localhost:5000';

// ── Clientes ──────────────────────────────────────────────
export const clientesApi = {
  obtenerTodos: async () => {
    const res = await fetch(`${API_BASE_URL}/api/clientes`);
    if (!res.ok) throw new Error('Error al obtener clientes');
    return res.json();
  },

  obtenerPorCedula: async (cedula) => {
    const res = await fetch(`${API_BASE_URL}/api/clientes/cedula/${cedula}`);
    if (!res.ok) return null;
    return res.json();
  },

  obtenerPorId: async (id) => {
    const res = await fetch(`${API_BASE_URL}/api/clientes/${id}`);
    if (!res.ok) return null;
    return res.json();
  }
};

// ── Productos ─────────────────────────────────────────────
export const productosApi = {
  obtenerTodos: async () => {
    const res = await fetch(`${API_BASE_URL}/api/productos`);
    if (!res.ok) throw new Error('Error al obtener productos');
    return res.json();
  },

  obtenerPorId: async (id) => {
    const res = await fetch(`${API_BASE_URL}/api/productos/${id}`);
    if (!res.ok) return null;
    return res.json();
  },

  buscarPorNombre: async (nombre) => {
    const res = await fetch(`${API_BASE_URL}/api/productos/buscar?nombre=${encodeURIComponent(nombre)}`);
    if (!res.ok) throw new Error('Error al buscar productos');
    return res.json();
  }
};

// ── Ventas ────────────────────────────────────────────────
export const ventasApi = {
  obtenerTodos: async () => {
    const res = await fetch(`${API_BASE_URL}/api/ventas`);
    if (!res.ok) throw new Error('Error al obtener ventas');
    return res.json();
  },

  crear: async (venta) => {
    const res = await fetch(`${API_BASE_URL}/api/ventas`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(venta)
    });
    if (!res.ok) {
      const error = await res.text();
      throw new Error(error || 'Error al crear la venta');
    }
    return res.json();
  }
};
