import { useRef, useEffect } from 'react'
import { Link } from 'react-router-dom'
import { useAuthStore } from '../store/authStore'
import '../pages/hero.css'

const STATS = [
  { num: '12k+', lbl: 'Cuidadores verificados' },
  { num: '80k+', lbl: 'Mascotas atendidas' },
  { num: '4.9★', lbl: 'Calificación promedio' },
  { num: '24/7', lbl: 'Soporte y seguro' },
]

export default function HeroSection() {
  const rootRef = useRef<HTMLDivElement>(null)
  const { token, role } = useAuthStore()

  const dashboardPath =
    role === 'Owner' ? '/owner/dashboard' :
    role === 'Herder' ? '/herder/dashboard' :
    role === 'Admin' ? '/admin/dashboard' : null

  useEffect(() => {
    const root = rootRef.current
    if (!root) return

    // Build layers list from data-depth elements
    const layers = [...root.querySelectorAll<HTMLElement>('[data-depth]')]
    let mx = 0, my = 0, sy = 0, raf: number | null = null

    const apply = () => {
      raf = null
      for (const l of layers) {
        const d  = parseFloat(l.getAttribute('data-depth')  ?? '0') || 0
        const sd = parseFloat(l.getAttribute('data-scroll') ?? '0') || 0
        const tx = (mx * d).toFixed(2)
        const ty = (my * d + sy * sd).toFixed(2)
        l.style.transform = `translate3d(${tx}px,${ty}px,0)`
      }
    }

    const schedule = () => { if (!raf) raf = requestAnimationFrame(apply) }

    const onMove = (e: MouseEvent) => {
      mx = e.clientX - window.innerWidth  / 2
      my = e.clientY - window.innerHeight / 2
      schedule()
    }
    const onScroll = () => { sy = window.scrollY; schedule() }

    window.addEventListener('mousemove', onMove)
    window.addEventListener('scroll',    onScroll, { passive: true })

    // Kick off initial frame so elements start in final composited state
    schedule()

    return () => {
      window.removeEventListener('mousemove', onMove)
      window.removeEventListener('scroll',    onScroll)
      if (raf) cancelAnimationFrame(raf)
    }
  }, [])

  return (
    <div ref={rootRef} className="hero-root">

      {/* ── HERO ── */}
      <section className="hero-section">

        {/* Blobs */}
        <div className="hero-blob hero-blob-teal"   data-depth="0.018" data-scroll="-0.07" />
        <div className="hero-blob hero-blob-orange" data-depth="0.030" data-scroll="-0.12" />
        <div className="hero-blob hero-blob-coral"  data-depth="0.022" data-scroll="-0.06" />

        {/* Paw prints */}
        <div className="hero-paw" data-depth="0.07" data-scroll="0.08"  style={{ top: '24%', left: '46%', fontSize: 38, opacity: 0.18 }}>🐾</div>
        <div className="hero-paw" data-depth="0.11" data-scroll="0.14"  style={{ top: '62%', left: '38%', fontSize: 26, opacity: 0.16 }}>🐾</div>
        <div className="hero-paw" data-depth="0.06" data-scroll="0.05"  style={{ top: '14%', right: '34%', fontSize: 30, opacity: 0.14 }}>🐾</div>

        {/* Top bar — subtle depth */}
        <div className="hero-topbar" data-depth="0.01">
          <Link to="/" className="hero-logo">
            <div className="hero-logo-icon">🐾</div>
            <span className="hero-logo-text">Nany<em>Pet</em></span>
          </Link>
          <nav className="hero-nav">
            <a href="#como-funciona" className="hero-nav-link">Cómo funciona</a>
            <a href="#por-que"       className="hero-nav-link">Por qué NanyPet</a>
            {token && dashboardPath ? (
              <Link to={dashboardPath} className="hero-nav-dashboard">Mi panel</Link>
            ) : (
              <>
                <Link to="/login"    className="hero-nav-link">Iniciar sesión</Link>
                <Link to="/register" className="hero-nav-login">Registrarse</Link>
              </>
            )}
          </nav>
        </div>

        {/* Body */}
        <div className="hero-body">

          {/* Copy — left */}
          <div className="hero-copy" data-depth="0.04" data-scroll="0.12">
            <div className="hero-eyebrow">
              <span className="hero-eyebrow-dot" />
              12,000+ cuidadores locales de confianza
            </div>
            <h1 className="hero-h1">
              Cuidadores que<br />
              tus mascotas van a <em>amar</em>
            </h1>
            <p className="hero-sub">
              Perros, gatos y toda criatura peluda, emplumada o escamosa — reserva
              cuidadores verificados cerca de ti en minutos. Bienvenido a NanyPet.
            </p>

            <div className="hero-search">
              <div className="hero-search-inner">
                <span className="hero-search-pin">📍</span>
                <input className="hero-search-input" placeholder="Tu barrio o ciudad" />
              </div>
              <Link to="/owner/dashboard" className="hero-search-btn">Buscar</Link>
            </div>

            <div className="hero-proof">
              <div className="hero-avatars">
                <div className="hero-avatar hero-avatar-1">🐕</div>
                <div className="hero-avatar hero-avatar-2">🐈</div>
                <div className="hero-avatar hero-avatar-3">🐰</div>
              </div>
              <div className="hero-proof-text">
                <strong>★ 4.9</strong> de 80k+ mascotas felices
              </div>
            </div>
          </div>

          {/* Pet stage — right
              Each card: data-depth + bob class on the SAME element so JS transform
              is the single source of truth, compositing cleanly with bob via CSS. */}
          <div className="hero-stage">

            {/* Dog */}
            <div
              className="hero-stage-layer hero-bob-slow"
              data-depth="0.14"
              data-scroll="0.18"
              style={{ top: '8%', left: '18%' }}
            >
              <div style={{ position: 'relative' }}>
                <div className="hero-pet-card hero-pet-dog">
                  <span className="emoji">🐕</span>
                  <span className="label">foto perro</span>
                </div>
                <div className="hero-dog-tag">Paseos diarios 🦮</div>
              </div>
            </div>

            {/* Cat */}
            <div
              className="hero-stage-layer hero-bob"
              data-depth="0.22"
              data-scroll="0.07"
              style={{ top: '2%', right: '2%' }}
            >
              <div className="hero-pet-card hero-pet-cat">
                <span className="emoji">🐈</span>
                <span className="label hero-label">foto gato</span>
              </div>
            </div>

            {/* Rabbit */}
            <div
              className="hero-stage-layer hero-bob hero-bob-d2"
              data-depth="0.30"
              data-scroll="-0.05"
              style={{ bottom: '14%', right: '6%' }}
            >
              <div className="hero-pet-card hero-pet-rabbit">
                <span className="emoji">🐰</span>
                <span className="label hero-label">conejo</span>
              </div>
            </div>

            {/* Bird */}
            <div
              className="hero-stage-layer hero-bob hero-bob-d1"
              data-depth="0.40"
              data-scroll="0.22"
              style={{ bottom: '2%', left: '6%' }}
            >
              <div className="hero-pet-card hero-pet-bird">
                <span className="emoji">🦜</span>
              </div>
            </div>

            {/* Booked badge */}
            <div
              className="hero-stage-layer"
              data-depth="0.34"
              data-scroll="0.13"
              style={{ top: '46%', left: 0 }}
            >
              <div className="hero-booked">
                <div className="hero-booked-icon">🏠</div>
                <div>
                  <div className="hero-booked-title">¡Reservado!</div>
                  <div className="hero-booked-sub">Maya está cuidando a Luna</div>
                </div>
              </div>
            </div>

          </div>
        </div>
      </section>

      {/* ── TRUST STRIP ── */}
      <section className="hero-trust">
        <div className="hero-trust-row" data-depth="0.025" data-scroll="0.08">
          {STATS.map(({ num, lbl }) => (
            <div key={lbl} className="hero-trust-stat">
              <p className="hero-trust-num">{num}</p>
              <p className="hero-trust-lbl">{lbl}</p>
            </div>
          ))}
        </div>
      </section>

    </div>
  )
}
