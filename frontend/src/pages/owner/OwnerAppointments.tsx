import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Link, useSearchParams } from 'react-router-dom'
import { useState } from 'react'
import { Map, Calendar, PawPrint, Plus, X, MessageCircle, CreditCard, CheckCircle } from 'lucide-react'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/owner/dashboard', icon: Map, label: 'Mapa' },
  { to: '/owner/appointments', icon: Calendar, label: 'Mis citas' },
  { to: '/owner/pets', icon: PawPrint, label: 'Mis mascotas' },
]

const STATUS_MAP: Record<number, { label: string; cls: string }> = {
  0: { label: 'Pendiente',  cls: 'badge-yellow' },
  1: { label: 'Confirmada', cls: 'badge-blue' },
  2: { label: 'En curso',   cls: 'badge-green' },
  3: { label: 'Completada', cls: 'badge-gray' },
  4: { label: 'Cancelada',  cls: 'badge-red' },
  5: { label: 'Rechazada',  cls: 'badge-red' },
}

export default function OwnerAppointments() {
  const qc = useQueryClient()
  const [params] = useSearchParams()
  const preselectedHerder = params.get('herder')
  const [showCreate, setShowCreate] = useState(!!preselectedHerder)
  const [form, setForm] = useState({ petId: '', herderId: preselectedHerder ?? '', appointmentTime: '', price: '', serviceType: '', notes: '' })

  const { data: appointments = [] } = useQuery({
    queryKey: ['appointments'],
    queryFn: () => api.get('/api/appointment').then((r) => r.data.result),
  })

  const { data: pets = [] } = useQuery({
    queryKey: ['pets'],
    queryFn: () => api.get('/api/pet').then((r) => r.data.result),
  })

  const createAppt = useMutation({
    mutationFn: (data: any) => api.post('/api/appointment', { ...data, price: Number(data.price) }),
    onSuccess: () => { qc.invalidateQueries({ queryKey: ['appointments'] }); setShowCreate(false) },
  })

  const cancelAppt = useMutation({
    mutationFn: (id: string) => api.put(`/api/appointment/${id}/cancel`, { reason: 'Cancelado por el dueño' }),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['appointments'] }),
  })

  const createPayment = useMutation({
    mutationFn: (id: string) => api.post('/api/payment', id, { headers: { 'Content-Type': 'application/json' } }),
    onSuccess: (res) => window.open(res.data.result.checkoutUrl, '_blank'),
  })

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header-row">
          <div className="dash-header">
            <h1>Mis citas</h1>
            <p>{appointments.length} cita{appointments.length !== 1 ? 's' : ''}</p>
          </div>
          <button onClick={() => setShowCreate(true)} className="dash-btn">
            <Plus style={{ width: 16, height: 16 }} /> Nueva cita
          </button>
        </div>

        {showCreate && (
          <div className="dash-form-panel">
            <div className="dash-form-panel-header">
              <h2 className="dash-form-panel-title">Solicitar cita</h2>
              <button onClick={() => setShowCreate(false)} className="dash-close-btn">
                <X style={{ width: 20, height: 20 }} />
              </button>
            </div>
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label">Mascota</label>
                <select value={form.petId} onChange={(e) => setForm({ ...form, petId: e.target.value })} className="dash-select">
                  <option value="">Selecciona una mascota</option>
                  {pets.map((p: any) => <option key={p.id} value={p.id}>{p.name}</option>)}
                </select>
              </div>
              <div className="dash-field">
                <label className="dash-label">ID del cuidador</label>
                <input value={form.herderId} onChange={(e) => setForm({ ...form, herderId: e.target.value })}
                  placeholder="ID del cuidador" className="dash-input" />
              </div>
            </div>
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label">Fecha y hora</label>
                <input type="datetime-local" value={form.appointmentTime}
                  onChange={(e) => setForm({ ...form, appointmentTime: e.target.value })} className="dash-input" />
              </div>
              <div className="dash-field">
                <label className="dash-label">Precio acordado (COP)</label>
                <input type="number" value={form.price}
                  onChange={(e) => setForm({ ...form, price: e.target.value })}
                  placeholder="Ej: 30000" className="dash-input" />
              </div>
            </div>
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label">Tipo de servicio</label>
                <select value={form.serviceType} onChange={(e) => setForm({ ...form, serviceType: e.target.value })} className="dash-select">
                  <option value="">Selecciona tipo</option>
                  <option value="Home">En mi casa</option>
                  <option value="Herder's Home">En casa del cuidador</option>
                  <option value="Park">En un parque</option>
                </select>
              </div>
              <div className="dash-field">
                <label className="dash-label">Notas</label>
                <input value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })}
                  placeholder="Instrucciones especiales..." className="dash-input" />
              </div>
            </div>
            <div style={{ display: 'flex', gap: 10, marginTop: 8 }}>
              <button onClick={() => createAppt.mutate(form)}
                disabled={createAppt.isPending || !form.petId || !form.herderId} className="dash-btn">
                {createAppt.isPending ? 'Enviando...' : 'Enviar solicitud'}
              </button>
              <button onClick={() => setShowCreate(false)} className="dash-btn-ghost">Cancelar</button>
            </div>
          </div>
        )}

        {appointments.length === 0 ? (
          <div className="dash-empty dash-card">
            <div className="dash-empty-icon">
              <Calendar style={{ width: 30, height: 30, color: '#f97316' }} />
            </div>
            <h3>Sin citas programadas</h3>
            <p>Busca un cuidador en el mapa y solicita tu primera cita</p>
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
                        : 'Sin fecha'}
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
                    <button onClick={() => cancelAppt.mutate(a.id)} className="appt-btn appt-btn-red">
                      Cancelar
                    </button>
                  )}
                  {a.status === 3 && (!a.paymentStatus || a.paymentStatus === 0) && (
                    <button onClick={() => createPayment.mutate(a.id)} className="appt-btn appt-btn-green">
                      <CreditCard style={{ width: 14, height: 14 }} /> Pagar
                    </button>
                  )}
                  {a.paymentStatus === 2 && (
                    <span className="appt-btn badge-green" style={{ display: 'inline-flex', alignItems: 'center', gap: 6 }}>
                      <CheckCircle style={{ width: 14, height: 14 }} /> Pago aprobado
                    </span>
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
