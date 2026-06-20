import { Link } from 'react-router-dom'
import {
  MapPin, Calendar, MessageCircle,
  Bell, CreditCard, ShieldCheck, Star, ArrowRight,
} from 'lucide-react'
import HeroSection from '../components/HeroSection'
import './landing.css'

export default function LandingPage() {
  return (
    <div>

      <HeroSection />

      {/* ── CÓMO FUNCIONA ── */}
      <div className="lp-section-white">
        <div className="lp-section-inner">
          <div className="lp-section-header">
            <p className="lp-section-eyebrow">Proceso simple</p>
            <h2 className="lp-section-title">¿Cómo funciona?</h2>
          </div>
          <div className="lp-grid-3">
            {[
              { icon: MapPin, num: '01', title: 'Encuentra un cuidador', desc: 'Usa tu ubicación para ver cuidadores verificados cerca con tarifas y calificaciones en tiempo real.', bg: 'linear-gradient(135deg,#f97316,#f59e0b)' },
              { icon: Calendar, num: '02', title: 'Agenda la cita', desc: 'Contacta al cuidador, define el precio y acuerda el horario. Todo dentro de la app.', bg: 'linear-gradient(135deg,#f97316,#f59e0b)' },
              { icon: Star, num: '03', title: 'Disfruta tranquilo', desc: 'Recibe actualizaciones, chatea en tiempo real y paga de forma segura al finalizar.', bg: 'linear-gradient(135deg,#f97316,#f59e0b)' },
            ].map(({ icon: Icon, num, title, desc, bg }) => (
              <div key={title} className="lp-card">
                <div className="lp-card-icon-row">
                  <div className="lp-icon-box" style={{ background: bg }}>
                    <Icon style={{ width: 24, height: 24, color: '#fff' }} />
                  </div>
                  <span className="lp-card-num">{num}</span>
                </div>
                <h3 className="lp-card-title">{title}</h3>
                <p className="lp-card-desc">{desc}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ── POR QUÉ NANYPET ── */}
      <div className="lp-section-gray">
        <div className="lp-section-inner">
          <div className="lp-section-header">
            <p className="lp-section-eyebrow">Todo incluido</p>
            <h2 className="lp-section-title">¿Por qué NanyPet?</h2>
          </div>
          <div className="lp-grid-6">
            {[
              { icon: ShieldCheck, title: 'Cuidadores verificados', desc: 'Verificación de identidad con foto y cédula antes de aparecer en el mapa.', bg: 'linear-gradient(135deg,#10b981,#22c55e)' },
              { icon: MapPin, title: 'Geolocalización', desc: 'Encuentra cuidadores en tu área exacta, ordenados por distancia en km.', bg: 'linear-gradient(135deg,#3b82f6,#6366f1)' },
              { icon: Star, title: 'Reseñas reales', desc: 'Calificaciones de dueños que ya usaron el servicio. Sin trampa.', bg: 'linear-gradient(135deg,#f59e0b,#f97316)' },
              { icon: MessageCircle, title: 'Chat en tiempo real', desc: 'Comunícate directamente con el cuidador durante el servicio vía SignalR.', bg: 'linear-gradient(135deg,#8b5cf6,#a855f7)' },
              { icon: Bell, title: 'Notificaciones push', desc: 'Alertas de confirmación, inicio y finalización del servicio en tu celular.', bg: 'linear-gradient(135deg,#ef4444,#ec4899)' },
              { icon: CreditCard, title: 'Pagos seguros', desc: 'Paga con Wompi: tarjetas, PSE y más. Sin complicaciones.', bg: 'linear-gradient(135deg,#14b8a6,#06b6d4)' },
            ].map(({ icon: Icon, title, desc, bg }) => (
              <div key={title} className="lp-card">
                <div className="lp-icon-box-sm" style={{ background: bg }}>
                  <Icon style={{ width: 20, height: 20, color: '#fff' }} />
                </div>
                <h3 className="lp-card-title">{title}</h3>
                <p className="lp-card-desc">{desc}</p>
              </div>
            ))}
          </div>
        </div>
      </div>

      {/* ── CTA ── */}
      <div className="lp-section-dark">
        <div style={{ maxWidth: 580, margin: '0 auto', textAlign: 'center' }}>
          <h2 className="lp-section-title-light">
            ¿Listo para salir sin preocupaciones?
          </h2>
          <p className="lp-section-sub-light">
            Regístrate gratis y encuentra el cuidador perfecto para tu mascota.
          </p>
          <Link to="/register" className="lp-btn-primary" style={{ fontSize: 18, padding: '16px 40px', borderRadius: 18 }}>
            Comenzar ahora <ArrowRight style={{ width: 22, height: 22 }} />
          </Link>
        </div>
      </div>

      {/* ── FOOTER ── */}
      <div className="lp-footer">
        <p>© {new Date().getFullYear()} NanyPet — Proyecto educativo</p>
      </div>

    </div>
  )
}
