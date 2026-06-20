import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { PawPrint, Mail, Lock, ArrowRight, Eye, EyeOff } from 'lucide-react'
import api from '../services/api'
import { useAuthStore } from '../store/authStore'
import './auth.css'

export default function LoginPage() {
  const navigate = useNavigate()
  const setAuth = useAuthStore(s => s.setAuth)
  const [form, setForm] = useState({ userName: '', password: '' })
  const [error, setError] = useState('')
  const [loading, setLoading] = useState(false)
  const [showPass, setShowPass] = useState(false)

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault(); setLoading(true); setError('')
    try {
      const res = await api.post('/api/signin', form)
      const { token, user } = res.data.result
      const payload = JSON.parse(atob(token.split('.')[1].replace(/-/g, '+').replace(/_/g, '/')))
      const role = payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] || payload.role || 'Owner'
      setAuth(user, token, role)
      if (role === 'Admin') navigate('/admin/dashboard')
      else if (role === 'Herder') navigate('/herder/dashboard')
      else navigate('/owner/dashboard')
    } catch { setError('Usuario o contraseña incorrectos.')
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
            Tu mascota merece<br />
            <span className="auth-panel-left-gradient">el mejor cuidado</span>
          </h2>
          <p className="auth-panel-left-sub">
            Conectamos dueños con cuidadores verificados. Seguro, simple y confiable.
          </p>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3,1fr)', gap: 12, marginTop: 32 }}>
            {[['500+', 'Cuidadores'], ['4.9★', 'Promedio'], ['100%', 'Verificados']].map(([v, l]) => (
              <div key={l} style={{ background: 'rgba(255,255,255,.06)', borderRadius: 14, padding: '14px 10px', border: '1px solid rgba(255,255,255,.08)', textAlign: 'center' }}>
                <p style={{ fontSize: 20, fontWeight: 800, color: '#fff', margin: 0 }}>{v}</p>
                <p style={{ fontSize: 11, color: '#64748b', margin: '4px 0 0', textTransform: 'uppercase', letterSpacing: '.06em' }}>{l}</p>
              </div>
            ))}
          </div>
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

          <h1 className="auth-title">Bienvenido de nuevo</h1>
          <p className="auth-subtitle">Inicia sesión en tu cuenta para continuar</p>

          <form onSubmit={handleSubmit}>
            <div className="auth-field">
              <label className="auth-label">Email</label>
              <div className="auth-input-wrap">
                <span className="auth-input-icon"><Mail style={iconSz} /></span>
                <input className="auth-input with-icon" type="email" required placeholder="tu@email.com"
                  value={form.userName} onChange={e => setForm({ ...form, userName: e.target.value })} />
              </div>
            </div>

            <div className="auth-field">
              <label className="auth-label">Contraseña</label>
              <div className="auth-input-wrap">
                <span className="auth-input-icon"><Lock style={iconSz} /></span>
                <input className="auth-input with-icon with-icon-r"
                  type={showPass ? 'text' : 'password'} required placeholder="••••••••"
                  value={form.password} onChange={e => setForm({ ...form, password: e.target.value })} />
                <button type="button" className="auth-input-eye" onClick={() => setShowPass(!showPass)}>
                  {showPass ? <EyeOff style={iconSz} /> : <Eye style={iconSz} />}
                </button>
              </div>
            </div>

            {error && <div className="auth-error">{error}</div>}

            <button type="submit" className="auth-btn" disabled={loading}>
              {loading ? 'Iniciando...' : <><span>Iniciar sesión</span><ArrowRight style={iconSz} /></>}
            </button>
          </form>

          <p className="auth-foot-text">
            ¿No tienes cuenta?{' '}
            <Link to="/register" className="auth-foot-link">Regístrate gratis</Link>
          </p>
        </div>
      </div>
    </div>
  )
}
