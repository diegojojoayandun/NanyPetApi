import React from 'react'
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom'
import { QueryClient, QueryClientProvider } from '@tanstack/react-query'
import { useAuthStore } from './store/authStore'

import LandingPage from './pages/LandingPage'
import LoginPage from './pages/LoginPage'
import RegisterPage from './pages/RegisterPage'

import OwnerDashboard from './pages/owner/OwnerDashboard'
import OwnerPets from './pages/owner/OwnerPets'
import OwnerAppointments from './pages/owner/OwnerAppointments'
import ChatPage from './pages/ChatPage'

import HerderDashboard from './pages/herder/HerderDashboard'
import HerderProfile from './pages/herder/HerderProfile'
import HerderAppointments from './pages/herder/HerderAppointments'
import HerderVerification from './pages/herder/HerderVerification'

import AdminDashboard from './pages/admin/AdminDashboard'
import AdminVerification from './pages/admin/AdminVerification'

const queryClient = new QueryClient()

function PrivateRoute({ children, role }: { children: React.ReactElement; role?: string }) {
  const { token, role: userRole } = useAuthStore()
  if (!token) return <Navigate to="/login" replace />
  if (role && userRole !== role) return <Navigate to="/" replace />
  return children
}

export default function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/" element={<LandingPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/register" element={<RegisterPage />} />

          {/* Owner */}
          <Route path="/owner/dashboard" element={<PrivateRoute role="Owner"><OwnerDashboard /></PrivateRoute>} />
          <Route path="/owner/pets" element={<PrivateRoute role="Owner"><OwnerPets /></PrivateRoute>} />
          <Route path="/owner/appointments" element={<PrivateRoute role="Owner"><OwnerAppointments /></PrivateRoute>} />

          {/* Herder */}
          <Route path="/herder/dashboard" element={<PrivateRoute role="Herder"><HerderDashboard /></PrivateRoute>} />
          <Route path="/herder/profile" element={<PrivateRoute role="Herder"><HerderProfile /></PrivateRoute>} />
          <Route path="/herder/appointments" element={<PrivateRoute role="Herder"><HerderAppointments /></PrivateRoute>} />
          <Route path="/herder/verification" element={<PrivateRoute role="Herder"><HerderVerification /></PrivateRoute>} />

          {/* Chat (owner y herder) */}
          <Route path="/chat/:appointmentId" element={<PrivateRoute><ChatPage /></PrivateRoute>} />

          {/* Admin */}
          <Route path="/admin/dashboard" element={<PrivateRoute role="Admin"><AdminDashboard /></PrivateRoute>} />
          <Route path="/admin/verification" element={<PrivateRoute role="Admin"><AdminVerification /></PrivateRoute>} />

          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  )
}
