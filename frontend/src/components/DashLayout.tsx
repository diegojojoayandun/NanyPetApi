import { useState } from 'react'
import { Link, useLocation, useNavigate } from 'react-router-dom'
import type { LucideIcon } from 'lucide-react'
import { LogOut, PawPrint, Menu } from 'lucide-react'
import { useAuthStore } from '../store/authStore'
import '../styles/dashboard.css'

export interface NavItem { to: string; icon: LucideIcon; label: string }

interface Props { items: NavItem[]; children: React.ReactNode }

export default function DashLayout({ items, children }: Props) {
  const [open, setOpen] = useState(false)
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const location = useLocation()

  return (
    <div className="dash">

      {/* ── SIDEBAR ── */}
      <aside className={`dash-sidebar${open ? ' open' : ''}`}>
        <div className="dash-logo">
          <Link to="/" className="dash-logo-row">
            <div className="dash-logo-icon">
              <PawPrint style={{ width: 16, height: 16, color: '#fff' }} />
            </div>
            <span className="dash-logo-text">NanyPet</span>
          </Link>
          <div className="dash-user">
            <div className="dash-user-avatar">
              {user?.firstName?.[0]}{user?.lastName?.[0]}
            </div>
            <div style={{ minWidth: 0 }}>
              <p className="dash-user-name">{user?.firstName} {user?.lastName}</p>
              <p className="dash-user-email">{user?.email ?? user?.userName}</p>
            </div>
          </div>
        </div>

        <nav className="dash-nav">
          {items.map(({ to, icon: Icon, label }) => {
            const active = location.pathname === to
            return (
              <Link key={to} to={to} onClick={() => setOpen(false)}
                className={`dash-nav-item${active ? ' active' : ''}`}>
                <Icon style={{ width: 17, height: 17 }} />
                {label}
              </Link>
            )
          })}
        </nav>

        <div className="dash-footer">
          <button className="dash-logout" onClick={() => { logout(); navigate('/') }}>
            <LogOut style={{ width: 17, height: 17 }} />
            Cerrar sesión
          </button>
        </div>
      </aside>

      {/* ── MOBILE OVERLAY ── */}
      <div
        className={`dash-overlay${open ? ' visible' : ''}`}
        onClick={() => setOpen(false)}
      />

      {/* ── MAIN ── */}
      <div className="dash-main">
        {/* Mobile top bar */}
        <div className="dash-topbar">
          <button className="dash-hamburger" onClick={() => setOpen(true)} aria-label="Abrir menú">
            <Menu style={{ width: 22, height: 22 }} />
          </button>
          <Link to="/" className="dash-topbar-logo">
            <div className="dash-topbar-logo-icon">
              <PawPrint style={{ width: 14, height: 14, color: '#fff' }} />
            </div>
            <span className="dash-topbar-logo-text">NanyPet</span>
          </Link>
        </div>

        {children}
      </div>
    </div>
  )
}
