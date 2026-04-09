import { useState, useEffect } from 'react';
import { clientesApi } from '../services/api';
import { Loader2, CreditCard, Phone, User, MapPin, Mail, Search } from 'lucide-react';
import './BuscadorCliente.css';

export default function BuscadorCliente({ onClienteSeleccionado, resetKey }) {
  const [cedula, setCedula] = useState('');
  const [buscando, setBuscando] = useState(false);
  const [error, setError] = useState('');
  
  // Estado para saber si estamos ingresando un cliente nuevo
  const [esNuevo, setEsNuevo] = useState(false);
  
  // Datos del cliente (ya sea encontrado o manual)
  const [datosCliente, setDatosCliente] = useState({
    id: 0,
    cedula: '',
    nombres: '',
    apellidos: '',
    direccion: '',
    telefono: '',
    email: ''
  });

  useEffect(() => {
  setCedula('');
  setEsNuevo(false);
  setError('');
  setDatosCliente({
    id: 0, cedula: '', nombres: '',
    apellidos: '', direccion: '', telefono: '', email: ''
  });
}, [resetKey]);

  // Efecto para propagar cambios en cliente manual
  useEffect(() => {
    if (esNuevo) {
      onClienteSeleccionado({
        ...datosCliente,
        cedula: cedula,
        esNuevo: true
      });
    }
  }, [datosCliente, esNuevo, cedula]);

  const handleChangeManual = (e) => {
    const { name, value } = e.target;
    setDatosCliente(prev => ({ ...prev, [name]: value }));
  };

  const buscarCliente = async () => {
    if (!cedula.trim()) {
      setError('Ingrese una cédula para buscar');
      return;
    }

    setBuscando(true);
    setError('');
    setEsNuevo(false);
    
    // Resetear datos previos
    setDatosCliente({
      id: 0,
      cedula: cedula,
      nombres: '',
      apellidos: '',
      direccion: '',
      telefono: '',
      email: ''
    });

    try {
      const resultado = await clientesApi.obtenerPorCedula(cedula.trim());
      if (resultado) {
        setDatosCliente(resultado);
        onClienteSeleccionado(resultado);
        setError('');
      } else {
        setError('Cliente no encontrado. Ingrese los datos para registrarlo.');
        setEsNuevo(true);
      }
    } catch (err) {
      setError('Error de conexión con el servidor. Ingrese datos manualmente.');
      setEsNuevo(true);
    } finally {
      setBuscando(false);
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === 'Enter') buscarCliente();
  };

  return (
    <div className="section-container">
      <div className="section-group-label">DATOS DEL CLIENTE</div>
      <div className="buscador-cliente-wrapper">
        <div className="cliente-fields-grid">
          <div className="field-row">
            <div className="field-item">
              <label><CreditCard size={14} className="label-icon" /> Cédula/Ruc:</label>
              <input
                type="text"
                value={cedula}
                onChange={(e) => {
                   setCedula(e.target.value);
                   setEsNuevo(false); 
                   onClienteSeleccionado(null);
                }}
                onKeyDown={handleKeyDown}
                className="field-editable input-icon-pad"
                placeholder=""
              />
            </div>
            <div className="field-item">
              <label><Phone size={14} className="label-icon" /> Teléfono:</label>
              <input 
                type="text" 
                name="telefono"
                value={datosCliente.telefono || ''} 
                onChange={handleChangeManual}
                readOnly={!esNuevo} 
                className={esNuevo ? "field-editable" : "field-readonly"} 
              />
            </div>
          </div>

          <div className="field-row">
            <div className="field-item">
              <label><User size={14} className="label-icon" /> Apellidos:</label>
              <input 
                type="text" 
                name="apellidos"
                value={datosCliente.apellidos || ''} 
                onChange={handleChangeManual}
                readOnly={!esNuevo} 
                className={esNuevo ? "field-editable" : "field-readonly"} 
              />
            </div>
            <div className="field-item">
              <label><MapPin size={14} className="label-icon" /> Dirección:</label>
              <input 
                type="text" 
                name="direccion"
                value={datosCliente.direccion || ''} 
                onChange={handleChangeManual}
                readOnly={!esNuevo} 
                className={esNuevo ? "field-editable" : "field-readonly"} 
              />
            </div>
          </div>

          <div className="field-row">
            <div className="field-item">
              <label><User size={14} className="label-icon" /> Nombres:</label>
              <input 
                type="text" 
                name="nombres"
                value={datosCliente.nombres || ''} 
                onChange={handleChangeManual}
                readOnly={!esNuevo} 
                className={esNuevo ? "field-editable" : "field-readonly"} 
              />
            </div>
            <div className="field-item">
              <label><Mail size={14} className="label-icon" /> Correo:</label>
              <input 
                type="text" 
                name="email"
                value={datosCliente.email || ''} 
                onChange={handleChangeManual}
                readOnly={!esNuevo} 
                className={esNuevo ? "field-editable" : "field-readonly"} 
              />
            </div>
          </div>
          {error && <div className="error-text-bottom">{error}</div>}
        </div>

        <div className="cliente-visual-icon">
          <button 
            type="button" 
            className="btn-visual-icon-search" 
            onClick={buscarCliente}
            disabled={buscando}
            title="Haga clic para buscar cliente"
          >
            {buscando ? (
              <Loader2 className="lucide-spin icon-search-blue" size={40} />
            ) : (
              <Search className="icon-search-blue" size={40} strokeWidth={2} />
            )}
          </button>
        </div>
      </div>
    </div>
  );
}
