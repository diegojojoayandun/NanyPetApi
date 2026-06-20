import { useEffect, useState } from 'react'
import { MapContainer, TileLayer, Marker, Popup, useMap } from 'react-leaflet'
import L from 'leaflet'
import 'leaflet/dist/leaflet.css'
import { useQuery } from '@tanstack/react-query'
import { Link } from 'react-router-dom'
import { Map, Calendar, PawPrint, Search, Star } from 'lucide-react'
import api from '../../services/api'
import { useLocationStore } from '../../store/locationStore'
import DashLayout from '../../components/DashLayout'

delete (L.Icon.Default.prototype as any)._getIconUrl
L.Icon.Default.mergeOptions({
  iconRetinaUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon-2x.png',
  iconUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-icon.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
})

const herderIcon = new L.Icon({
  iconUrl: 'https://raw.githubusercontent.com/pointhi/leaflet-color-markers/master/img/marker-icon-2x-green.png',
  shadowUrl: 'https://cdnjs.cloudflare.com/ajax/libs/leaflet/1.9.4/images/marker-shadow.png',
  iconSize: [25, 41], iconAnchor: [12, 41], popupAnchor: [1, -34], shadowSize: [41, 41],
})

function RecenterMap({ lat, lng }: { lat: number; lng: number }) {
  const map = useMap()
  useEffect(() => { map.setView([lat, lng], 13) }, [lat, lng])
  return null
}

const NAV_ITEMS = [
  { to: '/owner/dashboard', icon: Map, label: 'Mapa' },
  { to: '/owner/appointments', icon: Calendar, label: 'Mis citas' },
  { to: '/owner/pets', icon: PawPrint, label: 'Mis mascotas' },
]

export default function OwnerDashboard() {
  const { userLat, userLng, nearbyHerders, setUserLocation, setNearbyHerders } = useLocationStore()
  const [radius, setRadius] = useState(10)

  useEffect(() => {
    navigator.geolocation.getCurrentPosition(
      (pos) => setUserLocation(pos.coords.latitude, pos.coords.longitude),
      () => setUserLocation(4.7110, -74.0721)
    )
  }, [])

  const { isLoading, refetch } = useQuery({
    queryKey: ['nearby-herders', userLat, userLng, radius],
    queryFn: async () => {
      const res = await api.get('/api/herder/nearby', {
        params: { latitude: userLat, longitude: userLng, radiusKm: radius }
      })
      setNearbyHerders(res.data.result)
      return res.data.result
    },
    enabled: !!userLat && !!userLng,
  })

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page-wide">

        {/* Controls bar */}
        <div className="dash-map-controls">
          <label style={{ fontSize: 13, fontWeight: 600, color: '#475569' }}>Radio:</label>
          <select value={radius} onChange={(e) => setRadius(Number(e.target.value))}
            className="dash-select" style={{ width: 'auto', padding: '6px 12px' }}>
            {[5, 10, 20, 30].map((r) => <option key={r} value={r}>{r} km</option>)}
          </select>
          <button onClick={() => refetch()} className="dash-btn" style={{ padding: '7px 16px', fontSize: 13 }}>
            <Search style={{ width: 14, height: 14 }} />
            {isLoading ? 'Buscando...' : 'Buscar'}
          </button>
          <span className="dash-map-count">
            <strong>{nearbyHerders.length}</strong> cuidador{nearbyHerders.length !== 1 ? 'es' : ''} encontrado{nearbyHerders.length !== 1 ? 's' : ''}
          </span>
        </div>

        {/* Map + side panel */}
        <div className="dash-map-wrap">
          <div className="dash-map-main">
            <MapContainer
              center={[userLat ?? 4.7110, userLng ?? -74.0721]}
              zoom={13}
              style={{ height: '100%', width: '100%' }}
            >
              <TileLayer
                url="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png"
                attribution='© <a href="https://openstreetmap.org">OpenStreetMap</a>'
              />
              {userLat && userLng && (
                <>
                  <RecenterMap lat={userLat} lng={userLng} />
                  <Marker position={[userLat, userLng]}>
                    <Popup>Tu ubicación</Popup>
                  </Marker>
                </>
              )}
              {nearbyHerders.map((h) =>
                h.latitude && h.longitude ? (
                  <Marker key={h.id} position={[h.latitude, h.longitude]} icon={herderIcon}>
                    <Popup>
                      <div style={{ fontSize: 13, minWidth: 140 }}>
                        <p style={{ fontWeight: 700, color: '#0f172a', marginBottom: 4 }}>{h.emailUser}</p>
                        <p style={{ color: '#f59e0b', fontSize: 12, marginBottom: 2 }}>
                          {'★'.repeat(Math.round(h.averageRating ?? 0))} ({h.totalReviews ?? 0})
                        </p>
                        <p style={{ color: '#f97316', fontWeight: 700 }}>
                          ${h.hourlyRate?.toLocaleString('es-CO')}/hr
                        </p>
                        <p style={{ color: '#94a3b8', fontSize: 11, marginBottom: 8 }}>{h.distanceKm} km</p>
                        <Link to={`/owner/appointments?herder=${h.id}`}
                          style={{ display: 'block', textAlign: 'center', background: '#f97316', color: '#fff', padding: '5px 10px', borderRadius: 8, fontSize: 12, fontWeight: 700, textDecoration: 'none' }}>
                          Solicitar cita
                        </Link>
                      </div>
                    </Popup>
                  </Marker>
                ) : null
              )}
            </MapContainer>
          </div>

          {/* Herder list */}
          <div className="dash-map-panel">
            <div className="dash-map-panel-header">
              Cuidadores cercanos ({nearbyHerders.length})
            </div>
            <div className="dash-map-panel-list">
              {nearbyHerders.length === 0 ? (
                <div style={{ padding: '40px 16px', textAlign: 'center' }}>
                  <div className="dash-empty-icon" style={{ margin: '0 auto 12px' }}>
                    <Search style={{ width: 24, height: 24, color: '#f97316' }} />
                  </div>
                  <p style={{ fontSize: 13, fontWeight: 600, color: '#475569', margin: '0 0 4px' }}>Sin resultados</p>
                  <p style={{ fontSize: 12, color: '#94a3b8', margin: 0 }}>Aumenta el radio de búsqueda</p>
                </div>
              ) : (
                nearbyHerders.map((h) => (
                  <div key={h.id} className="dash-map-herder">
                    <div className="dash-map-herder-top">
                      <div className="dash-map-avatar">
                        {h.photoUrl
                          ? <img src={h.photoUrl} alt="" />
                          : (h.emailUser?.[0] ?? 'C').toUpperCase()}
                      </div>
                      <div style={{ flex: 1, minWidth: 0 }}>
                        <p className="dash-map-name">{h.emailUser}</p>
                        <p className="dash-map-rating">
                          <Star style={{ width: 11, height: 11, display: 'inline', fill: '#f59e0b', color: '#f59e0b', marginRight: 3 }} />
                          {(h.averageRating ?? 0).toFixed(1)} · {h.totalReviews ?? 0} reseñas
                        </p>
                      </div>
                      <div style={{ textAlign: 'right', flexShrink: 0 }}>
                        <p className="dash-map-price">${h.hourlyRate?.toLocaleString('es-CO')}</p>
                        <p className="dash-map-dist">{h.distanceKm} km</p>
                      </div>
                    </div>
                    <Link to={`/owner/appointments?herder=${h.id}`} className="dash-map-req-btn">
                      Solicitar cita
                    </Link>
                  </div>
                ))
              )}
            </div>
          </div>
        </div>

      </div>
    </DashLayout>
  )
}
