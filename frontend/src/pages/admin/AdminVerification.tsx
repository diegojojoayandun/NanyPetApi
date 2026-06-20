import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { LayoutDashboard, ShieldCheck, CheckCircle, XCircle, ExternalLink, Clock } from 'lucide-react'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/admin/dashboard', icon: LayoutDashboard, label: 'Dashboard' },
  { to: '/admin/verification', icon: ShieldCheck, label: 'Verificaciones' },
]

export default function AdminVerification() {
  const qc = useQueryClient()

  const { data: herders = [], isLoading } = useQuery({
    queryKey: ['admin-verification'],
    queryFn: () => api.get('/api/admin/herders/verification').then((r) => r.data.result),
  })

  const verify = useMutation({
    mutationFn: ({ id, action, reason }: { id: string; action: string; reason?: string }) =>
      api.put(`/api/admin/herders/${id}/${action}`, reason ? { reason } : undefined),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['admin-verification'] }),
  })

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page" style={{ maxWidth: 900 }}>

        <div className="dash-header">
          <h1>Verificación de cuidadores</h1>
          <p>Revisa los documentos y aprueba o rechaza cada solicitud</p>
        </div>

        {isLoading ? (
          <div style={{ textAlign: 'center', padding: '64px 24px', color: '#94a3b8', fontSize: 14 }}>Cargando solicitudes...</div>
        ) : herders.length === 0 ? (
          <div className="dash-empty dash-card">
            <div className="dash-empty-icon" style={{ background: '#f0fdf4' }}>
              <ShieldCheck style={{ width: 30, height: 30, color: '#10b981' }} />
            </div>
            <h3>Todo al día</h3>
            <p>No hay solicitudes de verificación pendientes</p>
          </div>
        ) : (
          herders.map((h: any) => (
            <div key={h.id} className="dash-card" style={{ marginBottom: 16, padding: 0, overflow: 'hidden' }}>
              {/* Header */}
              <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', padding: '14px 20px', borderBottom: '1px solid #f1f5f9', background: '#fafafa' }}>
                <div style={{ display: 'flex', alignItems: 'center', gap: 12 }}>
                  <div style={{
                    width: 36, height: 36, borderRadius: 10, flexShrink: 0,
                    background: 'linear-gradient(135deg,#f97316,#f59e0b)',
                    display: 'flex', alignItems: 'center', justifyContent: 'center',
                    fontWeight: 800, fontSize: 14, color: '#fff'
                  }}>
                    {(h.emailUser?.[0] ?? 'H').toUpperCase()}
                  </div>
                  <div>
                    <p style={{ fontSize: 13, fontWeight: 700, color: '#0f172a', margin: '0 0 2px' }}>{h.emailUser}</p>
                    <p style={{ fontSize: 12, color: '#64748b', margin: 0 }}>{h.city}{h.state ? `, ${h.state}` : ''}</p>
                  </div>
                </div>
                <span className="badge badge-yellow">
                  <Clock style={{ width: 11, height: 11 }} /> En revisión
                </span>
              </div>

              {/* Documents */}
              <div style={{ padding: 20 }}>
                <p style={{ fontSize: 11, fontWeight: 700, color: '#94a3b8', textTransform: 'uppercase', letterSpacing: '.06em', margin: '0 0 14px' }}>
                  Documentos
                </p>
                <div className="admin-doc-grid">
                  {[
                    { label: 'Foto de perfil',    url: h.photoUrl },
                    { label: 'Cédula (frente)',   url: h.idDocumentFrontUrl },
                    { label: 'Cédula (reverso)',  url: h.idDocumentBackUrl },
                    { label: 'Selfie con cédula', url: h.selfieWithIdUrl },
                  ].map(({ label, url }) => (
                    <div key={label}>
                      <p style={{ fontSize: 12, fontWeight: 600, color: '#475569', margin: '0 0 6px' }}>{label}</p>
                      {url ? (
                        <a href={url} target="_blank" rel="noopener noreferrer"
                          style={{ display: 'block', position: 'relative' }}>
                          <img src={url} alt={label} className="admin-doc-img" />
                          <div style={{ position: 'absolute', inset: 0, background: 'rgba(0,0,0,0)', display: 'flex', alignItems: 'center', justifyContent: 'center', transition: 'background .15s', borderRadius: 12 }}
                            onMouseOver={(e) => (e.currentTarget.style.background = 'rgba(0,0,0,.35)')}
                            onMouseOut={(e) => (e.currentTarget.style.background = 'transparent')}>
                            <ExternalLink style={{ width: 18, height: 18, color: '#fff', opacity: 0 }}
                              onMouseOver={(e) => (e.currentTarget.style.opacity = '1')}
                              onMouseOut={(e) => (e.currentTarget.style.opacity = '0')} />
                          </div>
                        </a>
                      ) : (
                        <div className="admin-doc-empty">
                          <p style={{ fontSize: 11, color: '#94a3b8', margin: 0 }}>No subido</p>
                        </div>
                      )}
                    </div>
                  ))}
                </div>

                <div style={{ display: 'flex', gap: 10 }}>
                  <button onClick={() => verify.mutate({ id: h.id, action: 'approve' })}
                    disabled={verify.isPending}
                    className="dash-btn" style={{ background: 'linear-gradient(90deg,#10b981,#22c55e)', boxShadow: '0 4px 12px rgba(16,185,129,.3)' }}>
                    <CheckCircle style={{ width: 15, height: 15 }} /> Aprobar
                  </button>
                  <button onClick={() => {
                    const r = prompt('Motivo del rechazo:')
                    if (r) verify.mutate({ id: h.id, action: 'reject', reason: r })
                  }} disabled={verify.isPending}
                    style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: '#fef2f2', color: '#dc2626', border: '1px solid #fecaca', borderRadius: 12, padding: '10px 20px', fontSize: 14, fontWeight: 700, cursor: 'pointer', fontFamily: 'inherit' }}>
                    <XCircle style={{ width: 15, height: 15 }} /> Rechazar
                  </button>
                </div>
              </div>
            </div>
          ))
        )}

      </div>
    </DashLayout>
  )
}
