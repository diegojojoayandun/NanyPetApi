import { useState } from 'react'
import { Link, useNavigate, useSearchParams } from 'react-router-dom'
import { PawPrint, User, Mail, Lock, ArrowRight, Eye, EyeOff } from 'lucide-react'
import api from '../services/api'
import './auth.css'

export default function RegisterPage() {
  const navigate = useNavigate()
  const [params] = useSearchParams()
  const [role, setRole] = useState<'Owner' | 'Herder'>(
    params.get('role') === 'Herder' ? 'Herder' : 'Owner'
  )
  const [form, setForm] = useState({ firstName: '', lastName: '', email: '', password: '', confirmPassword: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [showPass, setShowPass] = useState(false)

  const f = (k: keyof typeof form, v: string) => setForm({ ...form, [k]: v })

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (form.password !== form.confirmPassword) { setError('Las contraseñas no coinciden.'); return }
    setLoading(true); setError('')
    try {
      await api.post('/api/signup', { firstName: form.firstName, lastName: form.lastName, email: form.email, password: form.password, rol: role })
      navigate('/login')
    } catch (err: any) {
      setError(err.response?.data?.errorMessages?.[0] ?? 'Error al registrarse.')
    } finally { setLoading(false) }
  }

  const iconSz = { width: 16, height: 16 }

  return (
    <div className="auth-wrap">

      {/* LEFT PANEL */}
      <div className="auth-panel-left">
        <Link to="/" className="auth-panel-left-logo">
          <div className="auth-panel-left-logo-icon"><PawPrint style={{ width: 20, height: 20, color: '#fff' }} /></div>
          <span className="auth-panel-left-logo-text">NanyPet</span>
        </Link>
        <div className="auth-panel-left-body">
          <h2 className="auth-panel-left-h2">
            Únete a la comunidad<br />
            <span className="auth-panel-left-gradient">más confiable</span>
          </h2>
          <p className="auth-panel-left-sub">
            Más de 500 cuidadores verificados esperan para cuidar tu mascota con amor y responsabilidad.
          </p>
          {['Verificación de identidad obligatoria', 'Chat en tiempo real durante el servicio', 'Pagos seguros con Wompi'].map(t => (
            <div className="auth-panel-left-bullet" key={t}>
              <div className="auth-panel-left-dot" />
              <span className="auth-panel-left-bullet-text">{t}</span>
            </div>
          ))}
        </div>
        <span className="auth-panel-left-foot">© {new Date().getFullYear()} NanyPet</span>
      </div>

      {/* RIGHT PANEL */}
      <div className="auth-panel-right">
        <div className="auth-form-box">
          <Link to="/" className="auth-mobile-logo">
            <div className="auth-mobile-logo-icon"><PawPrint style={{ width: 16, height: 16, color: '#fff' }} /></div>
            <span className="auth-mobile-logo-text">NanyPet</span>
          </Link>

          <h1 className="auth-title">Crear cuenta</h1>
          <p className="auth-subtitle">¿Cómo quieres usar NanyPet?</p>

          <div className="auth-role-toggle">
            <button type="button" className={`auth-role-btn${role === 'Owner' ? ' active' : ''}`} onClick={() => setRole('Owner')}>
              Soy dueño
            </button>
            <button type="button" className={`auth-role-btn${role === 'Herder' ? ' active' : ''}`} onClick={() => setRole('Herder')}>
              Soy cuidador
            </button>
          </div>

          <form onSubmit={handleSubmit}>
            <div className="auth-field-row">
              <div>
                <label className="auth-label">Nombre</label>
                <div className="auth-input-wrap">
                  <span className="auth-input-icon"><User style={iconSz} /></span>
                  <input className="auth-input with-icon" required placeholder="Juan"
                    value={form.firstName} onChange={e => f('firstName', e.target.value)} />
                </div>
              </div>
              <div>
                <label className="auth-label">Apellido</label>
                <input className="auth-input" required placeholder="García"
                  value={form.lastName} onChange={e => f('lastName', e.target.value)} />
              </div>
            </div>

            <div className="auth-field">
              <label className="auth-label">Email</label>
              <div className="auth-input-wrap">
                <span className="auth-input-icon"><Mail style={iconSz} /></span>
                <input className="auth-input with-icon" type="email" required placeholder="tu@email.com"
                  value={form.email} onChange={e => f('email', e.target.value)} />
              </div>
            </div>

            <div className="auth-field">
              <label className="auth-label">Contraseña</label>
              <div className="auth-input-wrap">
                <span className="auth-input-icon"><Lock style={iconSz} /></span>
                <input className="auth-input with-icon with-icon-r" type={showPass ? 'text' : 'password'}
                  required placeholder="Mínimo 8 caracteres"
                  value={form.password} onChange={e => f('password', e.target.value)} />
                <button type="button" className="auth-input-eye" onClick={() => setShowPass(!showPass)}>
                  {showPass ? <EyeOff style={iconSz} /> : <Eye style={iconSz} />}
                </button>
              </div>
            </div>

            <div className="auth-field">
              <label className="auth-label">Confirmar contraseña</label>
              <div className="auth-input-wrap">
                <span className="auth-input-icon"><Lock style={iconSz} /></span>
                <input className="auth-input with-icon" type="password" required placeholder="Repite tu contraseña"
                  value={form.confirmPassword} onChange={e => f('confirmPassword', e.target.value)} />
              </div>
            </div>

            {error && <div className="auth-error">{error}</div>}

            <button type="submit" className="auth-btn" disabled={loading}>
              {loading ? 'Creando cuenta...' : <><span>Crear cuenta</span><ArrowRight style={iconSz} /></>}
            </button>
          </form>

          <p className="auth-foot-text">
            ¿Ya tienes cuenta?{' '}
            <Link to="/login" className="auth-foot-link">Inicia sesión</Link>
          </p>
        </div>
      </div>
    </div>
  )
}
