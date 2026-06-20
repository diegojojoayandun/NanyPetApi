import { Link } from 'react-router-dom'
import { LayoutDashboard, ShieldCheck, Users, ArrowRight, UserCheck } from 'lucide-react'
import { useQuery } from '@tanstack/react-query'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/admin/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/admin/verification', icon: ShieldCheck, label: 'Verificaciones' },
]

const STAT_COLORS = [
  'linear-gradient(135deg,#f59e0b,#f97316)',
  'linear-gradient(135deg,#3b82f6,#6366f1)',
  'linear-gradient(135deg,#10b981,#22c55e)',
]

export default function AdminDashboard() {
  const { data: pendingHerders = [] } = useQuery({
    queryKey: ['admin-verification'],
    queryFn: () => api.get('/api/admin/herders/verification').then((r) => r.data.result ?? []),
  })

  const stats = [
    { label: 'Verificaciones pendientes', value: pendingHerders.length, icon: ShieldCheck },
    { label: 'Total cuidadores',          value: '—',                   icon: Users },
    { label: 'Aprobados hoy',             value: '—',                   icon: UserCheck },
  ]

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header">
          <h1>Panel de administración</h1>
          <p>Gestión de la plataforma NanyPet</p>
        </div>

        <div className="dash-stat-grid" style={{ gridTemplateColumns: 'repeat(3,1fr)' }}>
          {stats.map(({ label, value, icon: Icon }, i) => (
            <div key={label} className="dash-stat-card">
              <div className="dash-stat-icon" style={{ background: STAT_COLORS[i] }}>
                <Icon style={{ width: 20, height: 20, color: '#fff' }} />
              </div>
              <p className="dash-stat-val">{value}</p>
              <p className="dash-stat-lbl">{label}</p>
            </div>
          ))}
        </div>

        <div className="dash-grid-2">
          <Link to="/admin/verification"
            style={{ textDecoration: 'none' }}
            className="dash-card">
            <div style={{
              width: 48, height: 48, borderRadius: 16, marginBottom: 16,
              background: 'linear-gradient(135deg,#f59e0b,#f97316)',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              boxShadow: '0 4px 14px rgba(249,115,22,.3)'
            }}>
              <ShieldCheck style={{ width: 24, height: 24, color: '#fff' }} />
            </div>
            <div style={{ display: 'flex', alignItems: 'flex-start', justifyContent: 'space-between' }}>
              <div>
                <h2 style={{ fontSize: 15, fontWeight: 800, color: '#0f172a', margin: '0 0 6px' }}>Verificaciones pendientes</h2>
                <p style={{ fontSize: 13, color: '#64748b', margin: 0 }}>Revisar y aprobar documentos de cuidadores.</p>
                {pendingHerders.length > 0 && (
                  <span className="badge badge-yellow" style={{ marginTop: 10, display: 'inline-flex' }}>
                    {pendingHerders.length} en espera
                  </span>
                )}
              </div>
              <ArrowRight style={{ width: 18, height: 18, color: '#cbd5e1', flexShrink: 0 }} />
            </div>
          </Link>

          <div className="dash-card" style={{ opacity: 0.6 }}>
            <div style={{ width: 48, height: 48, borderRadius: 16, marginBottom: 16, background: '#f1f5f9', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
              <Users style={{ width: 24, height: 24, color: '#94a3b8' }} />
            </div>
            <h2 style={{ fontSize: 15, fontWeight: 800, color: '#0f172a', margin: '0 0 6px' }}>Gestión de usuarios</h2>
            <p style={{ fontSize: 13, color: '#64748b', margin: 0 }}>Ver y administrar todos los usuarios de la plataforma.</p>
            <span className="badge badge-gray" style={{ marginTop: 10, display: 'inline-flex' }}>Próximamente</span>
          </div>
        </div>

      </div>
    </DashLayout>
  )
}
