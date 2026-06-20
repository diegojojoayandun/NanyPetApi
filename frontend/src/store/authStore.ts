import { create } from 'zustand'
import { persist } from 'zustand/middleware'

export type UserRole = 'Owner' | 'Herder' | 'Admin'

interface AuthUser {
  id: string
  userName: string
  firstName?: string
  lastName?: string
  email?: string
}

interface AuthState {
  user: AuthUser | null
  token: string | null
  role: UserRole | null
  setAuth: (user: AuthUser, token: string, role: UserRole) => void
  logout: () => void
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      role: null,
      setAuth: (user, token, role) => {
        localStorage.setItem('token', token)
        set({ user, token, role })
      },
      logout: () => {
        localStorage.removeItem('token')
        set({ user: null, token: null, role: null })
      },
    }),
    { name: 'nanypet-auth' }
  )
)
