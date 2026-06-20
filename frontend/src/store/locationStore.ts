import { create } from 'zustand'

export interface NearbyHerder {
  id: string
  emailUser: string
  latitude?: number
  longitude?: number
  hourlyRate?: number
  averageRating: number
  totalReviews: number
  photoUrl?: string
  city?: string
  serviceRadius?: number
  distanceKm?: number
  verificationStatus: number
  isAvailable: boolean
}

interface LocationState {
  userLat: number | null
  userLng: number | null
  nearbyHerders: NearbyHerder[]
  setUserLocation: (lat: number, lng: number) => void
  setNearbyHerders: (herders: NearbyHerder[]) => void
}

export const useLocationStore = create<LocationState>((set) => ({
  userLat: null,
  userLng: null,
  nearbyHerders: [],
  setUserLocation: (lat, lng) => set({ userLat: lat, userLng: lng }),
  setNearbyHerders: (herders) => set({ nearbyHerders: herders }),
}))
