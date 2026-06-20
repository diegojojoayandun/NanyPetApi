import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { LayoutDashboard, Calendar, User, ShieldCheck, MessageCircle, CheckCircle, XCircle, Play, Flag } from 'lucide-react'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/herder/dashboard', icon: LayoutDashboard, label: 'Panel' },
  { to: '/herder/appointments', icon: Calendar, label: 'Citas' },
  { to: '/herder/profile', icon: User, label: 'Mi perfil' },
  { to: '/herder/verification', icon: ShieldCheck, label: 'Verificación' },
]

const STATUS_MAP: Record<number, { label: string; cls: string }> = {
  0: { label: 'Pendiente',  cls: 'badge-yellow' },
  1: { label: 'Confirmada', cls: 'badge-blue' },
  2: { label: 'En curso',   cls: 'badge-green' },
  3: { label: 'Completada', cls: 'badge-gray' },
  4: { label: 'Cancelada',  cls: 'badge-red' },
  5: { label: 'Rechazada',  cls: 'badge-red' },
}

export default function HerderAppointments() {
  const qc = useQueryClient()

  const { data: appointments = [], isLoading } = useQuery({
    queryKey: ['herder-appointments'],
    queryFn: () => api.get('/api/appointment').then((r) => r.data.result),
  })

  const action = useMutation({
    mutationFn: ({ id, act, reason }: { id: string; act: string; reason?: string }) =>
      api.put(`/api/appointment/${id}/${act}`, reason ? { reason } : undefined),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['herder-appointments'] }),
  })

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header">
          <h1>Gestión de citas</h1>
          <p>{appointments.length} cita{appointments.length !== 1 ? 's' : ''} en total</p>
        </div>

        {isLoading ? (
          <div style={{ textAlign: 'center', padding: '64px 24px', color: '#94a3b8', fontSize: 14 }}>Cargando...</div>
        ) : appointments.length === 0 ? (
          <div className="dash-empty dash-card">
            <div className="dash-empty-icon">
              <Calendar style={{ width: 30, height: 30, color: '#f97316' }} />
            </div>
            <h3>Sin citas aún</h3>
            <p>Completa tu perfil para aparecer en el mapa</p>
          </div>
        ) : (
          appointments.map((a: any) => {
            const s = STATUS_MAP[a.status] ?? STATUS_MAP[0]
            return (
              <div key={a.id} className="appt-card">
                <div className="appt-card-top">
                  <div>
                    <span className={`badge ${s.cls}`}>{s.label}</span>
                    <p className="appt-date">
                      {a.appointmentTime
                        ? new Date(a.appointmentTime).toLocaleString('es-CO', { dateStyle: 'medium', timeStyle: 'short' })
                        : 'Sin fecha especificada'}
                    </p>
                    {a.serviceType && <p className="appt-type">{a.serviceType}</p>}
                  </div>
                  <p className="appt-price">${(a.price ?? 0).toLocaleString('es-CO')}</p>
                </div>

                {a.notes && <p className="appt-notes">{a.notes}</p>}

                <div className="appt-actions">
                  <Link to={`/chat/${a.id}`} className="appt-btn appt-btn-blue">
                    <MessageCircle style={{ width: 14, height: 14 }} /> Chat
                  </Link>

                  {a.status === 0 && (
                    <>
                      <button onClick={() => action.mutate({ id: a.id, act: 'confirm' })}
                        className="appt-btn appt-btn-green">
                        <CheckCircle style={{ width: 14, height: 14 }} /> Confirmar
                      </button>
                      <button onClick={() => {
                        const r = prompt('Motivo del rechazo:')
                        if (r) action.mutate({ id: a.id, act: 'reject', reason: r })
                      }} className="appt-btn appt-btn-red">
                        <XCircle style={{ width: 14, height: 14 }} /> Rechazar
                      </button>
                    </>
                  )}

                  {a.status === 1 && (
                    <button onClick={() => action.mutate({ id: a.id, act: 'start' })}
                      className="appt-btn appt-btn-orange">
                      <Play style={{ width: 14, height: 14 }} /> Iniciar servicio
                    </button>
                  )}

                  {a.status === 2 && (
                    <button onClick={() => action.mutate({ id: a.id, act: 'complete' })}
                      className="appt-btn appt-btn-purple">
                      <Flag style={{ width: 14, height: 14 }} /> Completar servicio
                    </button>
                  )}
                </div>
              </div>
            )
          })
        )}

      </div>
    </DashLayout>
  )
}
