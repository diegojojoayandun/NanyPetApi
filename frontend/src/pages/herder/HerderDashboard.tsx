import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { LayoutDashboard, Calendar, User, ShieldCheck, Bell, ArrowRight } from 'lucide-react'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/herder/dashboard', icon: LayoutDashboard, label: 'Panel' },
  { to: '/herder/appointments', icon: Calendar, label: 'Citas' },
  { to: '/herder/profile', icon: User, label: 'Mi perfil' },
  { to: '/herder/verification', icon: ShieldCheck, label: 'Verificación' },
]

const STAT_COLORS = [
  'linear-gradient(135deg,#f59e0b,#f97316)',
  'linear-gradient(135deg,#3b82f6,#6366f1)',
  'linear-gradient(135deg,#10b981,#22c55e)',
  'linear-gradient(135deg,#8b5cf6,#a855f7)',
]

export default function HerderDashboard() {
  const { data: appointments = [] } = useQuery({
    queryKey: ['herder-appointments'],
    queryFn: () => api.get('/api/appointment').then((r) => r.data.result),
  })

  const pending   = appointments.filter((a: any) => a.status === 0)
  const confirmed = appointments.filter((a: any) => a.status === 1)
  const active    = appointments.filter((a: any) => a.status === 2)
  const completed = appointments.filter((a: any) => a.status === 3)
  const totalEarnings = completed.reduce((sum: number, a: any) => sum + (a.price ?? 0), 0)

  const stats = [
    { label: 'Solicitudes',  value: pending.length },
    { label: 'Confirmadas',  value: confirmed.length },
    { label: 'En curso',     value: active.length },
    { label: 'Completadas',  value: completed.length },
  ]

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header">
          <h1>Panel del cuidador</h1>
          <p>Resumen de tu actividad</p>
        </div>

        {/* Stats */}
        <div className="dash-stat-grid">
          {stats.map(({ label, value }, i) => (
            <div key={label} className="dash-stat-card">
              <div className="dash-stat-icon" style={{ background: STAT_COLORS[i] }}>
                <LayoutDashboard style={{ width: 20, height: 20, color: '#fff' }} />
              </div>
              <p className="dash-stat-val">{value}</p>
              <p className="dash-stat-lbl">{label}</p>
            </div>
          ))}
        </div>

        {/* Earnings */}
        <div className="earnings-card" style={{ marginBottom: 24 }}>
          <p className="earnings-lbl">Ingresos totales</p>
          <p className="earnings-val">${totalEarnings.toLocaleString('es-CO')}</p>
          <p className="earnings-sub">
            COP · {completed.length} servicio{completed.length !== 1 ? 's' : ''} completado{completed.length !== 1 ? 's' : ''}
          </p>
        </div>

        {/* Pending requests card */}
        <div className="dash-card">
          <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', marginBottom: 16, paddingBottom: 14, borderBottom: '1px solid #f1f5f9' }}>
            <div style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
              <Bell style={{ width: 16, height: 16, color: '#f59e0b' }} />
              <span style={{ fontSize: 14, fontWeight: 700, color: '#0f172a' }}>Solicitudes pendientes</span>
              {pending.length > 0 && (
                <span className="badge badge-yellow">{pending.length}</span>
              )}
            </div>
            <Link to="/herder/appointments"
              style={{ display: 'flex', alignItems: 'center', gap: 4, fontSize: 13, color: '#f97316', fontWeight: 600, textDecoration: 'none' }}>
              Ver todas <ArrowRight style={{ width: 13, height: 13 }} />
            </Link>
          </div>

          {pending.length === 0 ? (
            <p style={{ textAlign: 'center', color: '#94a3b8', fontSize: 13, padding: '24px 0' }}>
              No hay solicitudes pendientes
            </p>
          ) : pending.slice(0, 3).map((a: any) => (
            <div key={a.id} style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '12px 0', borderBottom: '1px solid #f8fafc' }}>
              <div>
                <p style={{ fontSize: 13, fontWeight: 600, color: '#0f172a', margin: '0 0 3px' }}>Nueva solicitud de cita</p>
                <p style={{ fontSize: 12, color: '#64748b', margin: 0 }}>
                  {a.appointmentTime
                    ? new Date(a.appointmentTime).toLocaleString('es-CO', { dateStyle: 'medium', timeStyle: 'short' })
                    : 'Sin fecha'} · ${(a.price ?? 0).toLocaleString('es-CO')}
                </p>
              </div>
              <Link to="/herder/appointments" className="appt-btn appt-btn-orange">
                Gestionar
              </Link>
            </div>
          ))}
        </div>

      </div>
    </DashLayout>
  )
}
