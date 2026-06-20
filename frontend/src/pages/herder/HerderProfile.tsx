import { useState } from 'react'
import { useMutation } from '@tanstack/react-query'
import { LayoutDashboard, Calendar, User, ShieldCheck, MapPin, CheckCircle, DollarSign, Radio } from 'lucide-react'
import api from '../../services/api'
import { useAuthStore } from '../../store/authStore'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/herder/dashboard', icon: LayoutDashboard, label: 'Panel' },
  { to: '/herder/appointments', icon: Calendar, label: 'Citas' },
  { to: '/herder/profile', icon: User, label: 'Mi perfil' },
  { to: '/herder/verification', icon: ShieldCheck, label: 'Verificación' },
]

export default function HerderProfile() {
  const user = useAuthStore((s) => s.user)
  const [form, setForm] = useState({ phone: '', address: '', city: '', state: '', hourlyRate: '', serviceRadius: '', isAvailable: true })
  const [locLoading, setLocLoading] = useState(false)
  const [lat, setLat] = useState<number | null>(null)
  const [lng, setLng] = useState<number | null>(null)
  const [saved, setSaved] = useState(false)

  const getLocation = () => {
    setLocLoading(true)
    navigator.geolocation.getCurrentPosition(
      (pos) => { setLat(pos.coords.latitude); setLng(pos.coords.longitude); setLocLoading(false) },
      () => setLocLoading(false)
    )
  }

  const updateProfile = useMutation({
    mutationFn: () => api.put(`/api/herder/${user?.id}`, {
      ...form,
      hourlyRate: Number(form.hourlyRate),
      serviceRadius: Number(form.serviceRadius),
      latitude: lat,
      longitude: lng,
    }),
    onSuccess: () => setSaved(true),
  })

  const f = (k: keyof typeof form, v: string) => setForm({ ...form, [k]: v })

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header">
          <h1>Mi perfil</h1>
          <p>Configura tu información para aparecer en el mapa</p>
        </div>

        <form onSubmit={(e) => { e.preventDefault(); updateProfile.mutate() }}
          style={{ display: 'flex', flexDirection: 'column', gap: 16 }}>

          {/* Contacto */}
          <div className="dash-card">
            <p style={{ fontSize: 11, fontWeight: 700, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '.06em', margin: '0 0 16px' }}>
              Información de contacto
            </p>
            <div className="dash-field-row">
              {([['phone', 'Teléfono'], ['address', 'Dirección'], ['city', 'Ciudad'], ['state', 'Departamento']] as const).map(([k, label]) => (
                <div key={k} className="dash-field">
                  <label className="dash-label">{label}</label>
                  <input value={(form as any)[k]} onChange={(e) => f(k, e.target.value)} className="dash-input" />
                </div>
              ))}
            </div>
          </div>

          {/* Servicio */}
          <div className="dash-card">
            <p style={{ fontSize: 11, fontWeight: 700, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '.06em', margin: '0 0 16px' }}>
              Configuración de servicio
            </p>
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label" style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <DollarSign style={{ width: 14, height: 14 }} /> Tarifa por hora (COP)
                </label>
                <input type="number" value={form.hourlyRate} onChange={(e) => f('hourlyRate', e.target.value)}
                  placeholder="Ej: 25000" className="dash-input" />
              </div>
              <div className="dash-field">
                <label className="dash-label" style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                  <Radio style={{ width: 14, height: 14 }} /> Radio de servicio (km)
                </label>
                <input type="number" value={form.serviceRadius} onChange={(e) => f('serviceRadius', e.target.value)}
                  placeholder="Ej: 5" className="dash-input" />
              </div>
            </div>

            <div style={{ display: 'flex', alignItems: 'center', gap: 12, padding: 14, background: '#f8fafc', borderRadius: 12, marginTop: 4 }}>
              <button type="button"
                onClick={() => setForm({ ...form, isAvailable: !form.isAvailable })}
                style={{
                  position: 'relative', width: 44, height: 24, borderRadius: 999, border: 'none', cursor: 'pointer', flexShrink: 0,
                  background: form.isAvailable ? '#10b981' : '#cbd5e1', transition: 'background .2s'
                }}>
                <span style={{
                  position: 'absolute', top: 2, width: 20, height: 20, borderRadius: '50%', background: '#fff',
                  left: form.isAvailable ? 22 : 2, transition: 'left .2s', boxShadow: '0 1px 4px rgba(0,0,0,.2)'
                }} />
              </button>
              <div>
                <p style={{ fontSize: 13, fontWeight: 600, color: '#0f172a', margin: '0 0 2px' }}>Disponible para citas</p>
                <p style={{ fontSize: 12, color: '#64748b', margin: 0 }}>
                  {form.isAvailable ? 'Apareces en el mapa de dueños' : 'No apareces en el mapa'}
                </p>
              </div>
            </div>
          </div>

          {/* Ubicación */}
          <div className="dash-card">
            <p style={{ fontSize: 11, fontWeight: 700, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '.06em', margin: '0 0 16px' }}>
              Ubicación
            </p>
            {lat && lng ? (
              <div style={{ display: 'flex', alignItems: 'center', gap: 10, background: '#f0fdf4', border: '1px solid #bbf7d0', borderRadius: 12, padding: 14, marginBottom: 12 }}>
                <CheckCircle style={{ width: 18, height: 18, color: '#10b981', flexShrink: 0 }} />
                <div>
                  <p style={{ fontSize: 13, fontWeight: 700, color: '#065f46', margin: '0 0 2px' }}>Ubicación capturada</p>
                  <p style={{ fontSize: 12, color: '#047857', margin: 0 }}>{lat.toFixed(6)}, {lng.toFixed(6)}</p>
                </div>
              </div>
            ) : (
              <div style={{ background: '#fffbeb', border: '1px solid #fde68a', borderRadius: 12, padding: 14, marginBottom: 12 }}>
                <p style={{ fontSize: 13, color: '#92400e', margin: 0 }}>Sin ubicación — los dueños no podrán encontrarte en el mapa.</p>
              </div>
            )}
            <button type="button" onClick={getLocation} disabled={locLoading}
              style={{ display: 'flex', alignItems: 'center', gap: 8, background: '#f1f5f9', color: '#334155', border: 'none', borderRadius: 12, padding: '10px 16px', fontSize: 13, fontWeight: 600, cursor: 'pointer', fontFamily: 'inherit' }}>
              <MapPin style={{ width: 15, height: 15 }} />
              {locLoading ? 'Obteniendo ubicación...' : 'Usar mi ubicación actual'}
            </button>
          </div>

          {saved && (
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, background: '#f0fdf4', border: '1px solid #bbf7d0', borderRadius: 12, padding: '12px 16px', fontSize: 13, fontWeight: 600, color: '#065f46' }}>
              <CheckCircle style={{ width: 15, height: 15 }} /> Perfil actualizado correctamente
            </div>
          )}

          <button type="submit" disabled={updateProfile.isPending} className="dash-btn" style={{ justifyContent: 'center' }}>
            {updateProfile.isPending ? 'Guardando...' : 'Guardar cambios'}
          </button>
        </form>

      </div>
    </DashLayout>
  )
}
