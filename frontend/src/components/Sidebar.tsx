import { Link, useLocation, useNavigate } from 'react-router-dom'
import type { LucideIcon } from 'lucide-react'
import { LogOut, PawPrint } from 'lucide-react'
import { useAuthStore } from '../store/authStore'

export interface NavItem { to: string; icon: LucideIcon; label: string }

export default function Sidebar({ items }: { items: NavItem[] }) {
  const { user, logout } = useAuthStore()
  const navigate = useNavigate()
  const location = useLocation()

  return (
    <aside className="w-64 bg-slate-900 flex flex-col h-screen sticky top-0 shrink-0">
      <div className="px-5 py-6 border-b border-slate-800">
        <div className="flex items-center gap-2.5 mb-5">
          <div className="w-8 h-8 bg-linear-to-br from-orange-500 to-amber-500 rounded-xl flex items-center justify-center shadow-lg shadow-orange-500/30">
            <PawPrint className="w-4 h-4 text-white" />
          </div>
          <span className="font-bold text-white text-lg tracking-tight">NanyPet</span>
        </div>
        <div className="flex items-center gap-3">
          <div className="w-9 h-9 rounded-xl bg-linear-to-br from-orange-400 to-amber-500 flex items-center justify-center text-white font-bold text-sm shadow-md shrink-0">
            {user?.firstName?.[0]}{user?.lastName?.[0]}
          </div>
          <div className="min-w-0">
            <p className="text-sm font-semibold text-white truncate">{user?.firstName} {user?.lastName}</p>
            <p className="text-xs text-slate-500 truncate">{user?.email ?? user?.userName}</p>
          </div>
        </div>
      </div>

      <nav className="flex-1 p-3 space-y-0.5 overflow-y-auto">
        {items.map(({ to, icon: Icon, label }) => {
          const active = location.pathname === to
          return (
            <Link key={to} to={to}
              className={`flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm font-medium transition-all duration-150 ${
                active
                  ? 'bg-linear-to-r from-orange-500 to-amber-500 text-white shadow-lg shadow-orange-500/20'
                  : 'text-slate-400 hover:bg-slate-800 hover:text-white'
              }`}>
              <Icon className="w-4 h-4 shrink-0" />
              {label}
            </Link>
          )
        })}
      </nav>

      <div className="p-3 border-t border-slate-800">
        <button onClick={() => { logout(); navigate('/') }}
          className="flex items-center gap-3 w-full px-3 py-2.5 text-slate-500 hover:text-red-400 hover:bg-slate-800 rounded-xl text-sm font-medium transition-all duration-150">
          <LogOut className="w-4 h-4" />
          Cerrar sesión
        </button>
      </div>
    </aside>
  )
}
