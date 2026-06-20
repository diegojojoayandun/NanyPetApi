import { useState } from 'react'
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query'
import { Map, Calendar, PawPrint, Plus, X, Trash2, AlertCircle, CheckCircle } from 'lucide-react'
import api from '../../services/api'
import DashLayout from '../../components/DashLayout'

interface Pet { id: string; name: string; species: string; breed: string; age: number; gender: string }

const NAV_ITEMS = [
  { to: '/owner/dashboard', icon: Map,      label: 'Mapa' },
  { to: '/owner/appointments', icon: Calendar, label: 'Mis citas' },
  { to: '/owner/pets', icon: PawPrint, label: 'Mis mascotas' },
]

const SPECIES_ICON: Record<string, string> = {
  Perro: '🐕', Gato: '🐈', Conejo: '🐇', Ave: '🦜', Pez: '🐟', Otro: '🐾',
}

const BREEDS: Record<string, string[]> = {
  Perro: [
    'Mestizo', 'Labrador Retriever', 'Golden Retriever', 'Pastor Alemán',
    'Bulldog Francés', 'Poodle', 'Chihuahua', 'Yorkshire Terrier', 'Beagle',
    'Husky Siberiano', 'Dálmata', 'Rottweiler', 'Doberman', 'Cocker Spaniel',
    'Pomerania', 'Bóxer', 'Border Collie', 'Shih Tzu', 'Maltés', 'Schnauzer',
    'Pitbull', 'Samoyedo', 'Akita', 'Corgi', 'Otro',
  ],
  Gato: [
    'Mestizo', 'Siamés', 'Persa', 'Maine Coon', 'Bengal', 'Ragdoll',
    'Esfinge', 'Británico de Pelo Corto', 'Scottish Fold', 'Abisinio',
    'Azul Ruso', 'Birmano', 'Angora Turco', 'Bombay', 'Otro',
  ],
  Conejo: [
    'Holland Lop', 'Mini Rex', 'Holandés', 'Rex', 'Angora', 'Lionhead',
    'Cabeza de León', 'Nueva Zelanda', 'Otro',
  ],
  Ave: [
    'Periquito', 'Canario', 'Cacatúa', 'Loro Amazónico', 'Agapornis',
    'Ninfa', 'Cotorra', 'Jilguero', 'Otro',
  ],
  Pez: ['Betta', 'Goldfish', 'Koi', 'Guppy', 'Neón', 'Molly', 'Otro'],
  Otro: ['No especificado'],
}

const firstBreed = (sp: string) => BREEDS[sp]?.[0] ?? 'Otro'
const INITIAL = { name: '', species: 'Perro', breed: firstBreed('Perro'), age: 1, gender: 'Macho' }

export default function OwnerPets() {
  const qc = useQueryClient()
  const [showForm, setShowForm] = useState(false)
  const [form, setForm]         = useState(INITIAL)
  const [apiError, setApiError] = useState<string | null>(null)
  const [success, setSuccess]   = useState(false)

  const { data: pets = [], isLoading } = useQuery<Pet[]>({
    queryKey: ['pets'],
    queryFn: () => api.get('/api/pet').then((r) => r.data.result ?? []),
  })

  const createPet = useMutation({
    mutationFn: (data: typeof form) => api.post('/api/pet', data),
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ['pets'] })
      setShowForm(false)
      setForm(INITIAL)
      setApiError(null)
      setSuccess(true)
      setTimeout(() => setSuccess(false), 3000)
    },
    onError: (err: any) => {
      const msgs: string[] =
        err?.response?.data?.errorMessages ??
        err?.response?.data?.ErrorMessages ??
        []
      setApiError(
        msgs.length > 0
          ? msgs.join(' · ')
          : `Error ${err?.response?.status ?? ''}: no se pudo guardar la mascota.`
      )
    },
  })

  const deletePet = useMutation({
    mutationFn: (id: string) => api.delete(`/api/pet/${id}`),
    onSuccess: () => qc.invalidateQueries({ queryKey: ['pets'] }),
  })

  const handleSpeciesChange = (sp: string) => {
    setForm({ ...form, species: sp, breed: firstBreed(sp) })
  }

  const breeds = BREEDS[form.species] ?? BREEDS.Otro
  const canSave = !!form.name.trim() && !!form.breed

  return (
    <DashLayout items={NAV_ITEMS}>
      <div className="dash-page">

        {/* Header */}
        <div className="dash-header-row">
          <div className="dash-header">
            <h1>Mis mascotas</h1>
            <p>{pets.length} mascota{pets.length !== 1 ? 's' : ''} registrada{pets.length !== 1 ? 's' : ''}</p>
          </div>
          <button onClick={() => { setShowForm(true); setApiError(null) }} className="dash-btn">
            <Plus style={{ width: 16, height: 16 }} /> Agregar mascota
          </button>
        </div>

        {/* Success toast */}
        {success && (
          <div style={{
            display: 'flex', alignItems: 'center', gap: 10,
            background: '#dcfce7', border: '1px solid #86efac',
            borderRadius: 12, padding: '12px 16px', marginBottom: 16,
            color: '#15803d', fontWeight: 700, fontSize: 14,
          }}>
            <CheckCircle style={{ width: 18, height: 18, flexShrink: 0 }} />
            Mascota guardada correctamente.
          </div>
        )}

        {/* New pet form */}
        {showForm && (
          <div className="dash-form-panel">
            <div className="dash-form-panel-header">
              <h2 className="dash-form-panel-title">Nueva mascota</h2>
              <button onClick={() => { setShowForm(false); setApiError(null) }} className="dash-close-btn">
                <X style={{ width: 20, height: 20 }} />
              </button>
            </div>

            {/* API error */}
            {apiError && (
              <div style={{
                display: 'flex', alignItems: 'flex-start', gap: 10,
                background: '#fef2f2', border: '1px solid #fca5a5',
                borderRadius: 10, padding: '12px 14px', marginBottom: 16,
                color: '#b91c1c', fontSize: 13, fontWeight: 600,
              }}>
                <AlertCircle style={{ width: 16, height: 16, flexShrink: 0, marginTop: 1 }} />
                <span>{apiError}</span>
              </div>
            )}

            {/* Row 1: name + species */}
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label">Nombre *</label>
                <input
                  type="text"
                  value={form.name}
                  onChange={(e) => setForm({ ...form, name: e.target.value })}
                  placeholder="Ej: Luna"
                  className="dash-input"
                />
              </div>
              <div className="dash-field">
                <label className="dash-label">Especie</label>
                <select
                  value={form.species}
                  onChange={(e) => handleSpeciesChange(e.target.value)}
                  className="dash-select"
                >
                  {Object.keys(BREEDS).map((s) => (
                    <option key={s}>{s}</option>
                  ))}
                </select>
              </div>
            </div>

            {/* Row 2: breed (dropdown) + gender */}
            <div className="dash-field-row">
              <div className="dash-field">
                <label className="dash-label">Raza</label>
                <select
                  value={form.breed}
                  onChange={(e) => setForm({ ...form, breed: e.target.value })}
                  className="dash-select"
                >
                  {breeds.map((b) => <option key={b}>{b}</option>)}
                </select>
              </div>
              <div className="dash-field">
                <label className="dash-label">Género</label>
                <select
                  value={form.gender}
                  onChange={(e) => setForm({ ...form, gender: e.target.value })}
                  className="dash-select"
                >
                  <option>Macho</option>
                  <option>Hembra</option>
                </select>
              </div>
            </div>

            {/* Age */}
            <div className="dash-field" style={{ maxWidth: 200 }}>
              <label className="dash-label">Edad (años)</label>
              <input
                type="number" min={0} max={50}
                value={form.age}
                onChange={(e) => setForm({ ...form, age: Number(e.target.value) })}
                className="dash-input"
              />
            </div>

            {/* Actions */}
            <div style={{ display: 'flex', gap: 10, marginTop: 16 }}>
              <button
                onClick={() => { setApiError(null); createPet.mutate(form) }}
                disabled={createPet.isPending || !canSave}
                className="dash-btn"
              >
                {createPet.isPending ? 'Guardando...' : 'Guardar mascota'}
              </button>
              <button onClick={() => { setShowForm(false); setApiError(null) }} className="dash-btn-ghost">
                Cancelar
              </button>
            </div>
          </div>
        )}

        {/* Pet list */}
        {isLoading ? (
          <div style={{ textAlign: 'center', padding: '64px 24px', color: '#94a3b8', fontSize: 14 }}>
            Cargando...
          </div>
        ) : pets.length === 0 ? (
          <div className="dash-empty dash-card">
            <div className="dash-empty-icon">
              <PawPrint style={{ width: 30, height: 30, color: '#f97316' }} />
            </div>
            <h3>Sin mascotas registradas</h3>
            <p>Agrega tu primera mascota para empezar</p>
          </div>
        ) : (
          <div className="pet-grid">
            {pets.map((pet) => (
              <div key={pet.id} className="pet-card">
                <div className="pet-card-top">
                  <div className="pet-emoji">{SPECIES_ICON[pet.species] ?? '🐾'}</div>
                  <div style={{ flex: 1, minWidth: 0 }}>
                    <div style={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                      <p className="pet-name">{pet.name}</p>
                      <button
                        onClick={() => { if (confirm(`¿Eliminar a ${pet.name}?`)) deletePet.mutate(pet.id) }}
                        title="Eliminar"
                        style={{ background: 'none', border: 'none', cursor: 'pointer', color: '#cbd5e1', padding: 4 }}
                        onMouseOver={(e) => (e.currentTarget.style.color = '#f87171')}
                        onMouseOut={(e)  => (e.currentTarget.style.color = '#cbd5e1')}
                      >
                        <Trash2 style={{ width: 15, height: 15 }} />
                      </button>
                    </div>
                    <p className="pet-breed">{pet.species} · {pet.breed}</p>
                    <div className="pet-tags">
                      <span className="pet-tag">{pet.age} años</span>
                      <span className="pet-tag">{pet.gender}</span>
                    </div>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

      </div>
    </DashLayout>
  )
}
