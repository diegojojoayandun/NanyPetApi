import { useState } from 'react'
import { Link } from 'react-router-dom'
import { LayoutDashboard, Calendar, User, ShieldCheck, Upload, CheckCircle, AlertCircle } from 'lucide-react'
import api from '../../services/api'
import { useAuthStore } from '../../store/authStore'
import DashLayout from '../../components/DashLayout'

const NAV_ITEMS = [
  { to: '/herder/dashboard', icon: LayoutDashboard, label: 'Panel' },
  { to: '/herder/appointments', icon: Calendar, label: 'Citas' },
  { to: '/herder/profile', icon: User, label: 'Mi perfil' },
  { to: '/herder/verification', icon: ShieldCheck, label: 'Verificación' },
]

type DocKey = 'photo' | 'idFront' | 'idBack' | 'selfieWithId'

const DOCS: { key: DocKey; label: string; hint: string }[] = [
  { key: 'photo',       label: 'Foto de perfil',    hint: 'Foto tuya con buena iluminación, fondo neutro' },
  { key: 'idFront',     label: 'Cédula (frente)',    hint: 'Frente del documento de identidad, legible' },
  { key: 'idBack',      label: 'Cédula (reverso)',   hint: 'Reverso del documento de identidad' },
  { key: 'selfieWithId', label: 'Selfie con cédula', hint: 'Foto tuya sosteniendo tu cédula visible' },
]

export default function HerderVerification() {
  const user = useAuthStore((s) => s.user)
  const [files, setFiles]     = useState<Partial<Record<DocKey, File>>>({})
  const [previews, setPreviews] = useState<Partial<Record<DocKey, string>>>({})
  const [loading, setLoading]   = useState(false)
  const [success, setSuccess]   = useState(false)
  const [error, setError]       = useState('')

  const handleFile = (key: DocKey, file?: File) => {
    if (!file) return
    setFiles((p) => ({ ...p, [key]: file }))
    setPreviews((p) => ({ ...p, [key]: URL.createObjectURL(file) }))
  }

  const allUploaded = DOCS.every(({ key }) => !!files[key])
  const done = Object.keys(files).length

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!user?.id) return
    setLoading(true); setError('')
    const fd = new FormData()
    DOCS.forEach(({ key }) => { if (files[key]) fd.append(key, files[key]!) })
    try {
      await api.post(`/api/herder/${user.id}/verification`, fd, { headers: { 'Content-Type': 'multipart/form-data' } })
      setSuccess(true)
    } catch {
      setError('Error al subir los documentos. Intenta de nuevo.')
    } finally { setLoading(false) }
  }

  if (success) {
    return (
      <DashLayout items={NAV_ITEMS}>
        <div className="dash-page" style={{ display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
          <div className="dash-card" style={{ maxWidth: 400, width: '100%', textAlign: 'center', padding: '48px 32px' }}>
            <div style={{
              width: 72, height: 72, borderRadius: 22, margin: '0 auto 20px',
              background: 'linear-gradient(135deg,#10b981,#22c55e)',
              display: 'flex', alignItems: 'center', justifyContent: 'center',
              boxShadow: '0 8px 24px rgba(16,185,129,.3)'
            }}>
              <CheckCircle style={{ width: 36, height: 36, color: '#fff' }} />
            </div>
            <h1 style={{ fontSize: 20, fontWeight: 800, color: '#0f172a', margin: '0 0 8px' }}>Documentos enviados</h1>
            <p style={{ fontSize: 14, color: '#64748b', lineHeight: 1.6, margin: '0 0 28px' }}>
              Tu perfil está en revisión. Te notificaremos cuando sea aprobado y podrás aparecer en el mapa.
            </p>
            <Link to="/herder/dashboard" className="dash-btn" style={{ justifyContent: 'center' }}>
              Volver al panel
            </Link>
          </div>
        </div>
      </DashLayout>
    )
  }

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        <div className="dash-header">
          <h1>Verificación de identidad</h1>
          <p>Sube los documentos requeridos para aparecer en el mapa</p>
        </div>

        {/* Info banner */}
        <div style={{ display: 'flex', gap: 12, background: 'rgba(249,115,22,.06)', border: '1px solid rgba(249,115,22,.2)', borderRadius: 14, padding: 16, marginBottom: 20 }}>
          <ShieldCheck style={{ width: 18, height: 18, color: '#f97316', flexShrink: 0, marginTop: 1 }} />
          <div>
            <p style={{ fontSize: 13, fontWeight: 700, color: '#9a3412', margin: '0 0 3px' }}>¿Por qué verificamos tu identidad?</p>
            <p style={{ fontSize: 13, color: '#c2410c', margin: 0, lineHeight: 1.5 }}>
              Para garantizar la seguridad de las mascotas y sus dueños, todos los cuidadores deben verificarse antes de aparecer en el mapa.
            </p>
          </div>
        </div>

        <form onSubmit={handleSubmit}>
          <div className="verif-grid">
            {DOCS.map(({ key, label, hint }) => (
              <div key={key} className="verif-doc-card">
                <div className="verif-doc-preview">
                  {previews[key] ? (
                    <>
                      <img src={previews[key]} alt={label} />
                      <div style={{ position: 'absolute', top: 8, right: 8, width: 24, height: 24, background: '#10b981', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                        <CheckCircle style={{ width: 14, height: 14, color: '#fff' }} />
                      </div>
                    </>
                  ) : (
                    <label style={{ display: 'flex', flexDirection: 'column', alignItems: 'center', gap: 6, cursor: 'pointer', width: '100%', height: '100%', justifyContent: 'center', background: '#f8fafc' }}>
                      <Upload style={{ width: 22, height: 22, color: '#94a3b8' }} />
                      <span style={{ fontSize: 12, color: '#94a3b8' }}>Subir imagen</span>
                      <input type="file" accept="image/*" style={{ display: 'none' }}
                        onChange={(e) => handleFile(key, e.target.files?.[0])} />
                    </label>
                  )}
                </div>
                <div className="verif-doc-info">
                  <p className="verif-doc-name">{label}</p>
                  <p className="verif-doc-hint">{hint}</p>
                  {previews[key] && (
                    <label style={{ display: 'flex', alignItems: 'center', gap: 4, fontSize: 12, color: '#f97316', cursor: 'pointer', fontWeight: 600, marginTop: 6 }}>
                      <Upload style={{ width: 11, height: 11 }} /> Cambiar
                      <input type="file" accept="image/*" style={{ display: 'none' }}
                        onChange={(e) => handleFile(key, e.target.files?.[0])} />
                    </label>
                  )}
                </div>
              </div>
            ))}
          </div>

          {/* Progress bar */}
          <div className="verif-progress" style={{ margin: '20px 0' }}>
            {DOCS.map(({ key }) => (
              <div key={key} className={`verif-progress-bar${files[key] ? ' done' : ''}`} />
            ))}
            <span style={{ fontSize: 12, color: '#64748b', flexShrink: 0 }}>{done}/4</span>
          </div>

          {error && (
            <div style={{ display: 'flex', alignItems: 'center', gap: 8, background: '#fef2f2', border: '1px solid #fecaca', borderRadius: 12, padding: '12px 14px', fontSize: 13, color: '#dc2626', marginBottom: 14 }}>
              <AlertCircle style={{ width: 15, height: 15, flexShrink: 0 }} /> {error}
            </div>
          )}

          <button type="submit" disabled={loading || !allUploaded} className="dash-btn" style={{ justifyContent: 'center' }}>
            {loading
              ? 'Subiendo documentos...'
              : !allUploaded
              ? `Faltan ${4 - done} documento${4 - done !== 1 ? 's' : ''}`
              : 'Enviar documentos para verificación'}
          </button>
        </form>

      </div>
    </DashLayout>
  )
}
