<template>
  <div class="events">
    <div class="header">
      <h2>Renginiai</h2>
      <div class="actions">
        <button class="secondary" type="button" @click="toggleMine">
          {{ showMine ? 'Rodyti visus' : 'Rodyti mano renginius' }}
        </button>
        <button class="primary" type="button" @click="goCreate">Sukurti savo rengini</button>
      </div>
    </div>

    <p v-if="loading" class="muted">Kraunama...</p>
    <p v-else-if="errorMessage" class="error">{{ errorMessage }}</p>

    <div v-else class="tableWrap">
      <table class="table">
        <thead>
          <tr>
            <th>Pavadinimas</th>
            <th>Vieta</th>
            <th>Savininkas</th>
            <th>Dalyviai</th>
          </tr>
        </thead>
        <tbody>
          <tr
            v-for="ev in filteredEvents"
            :key="ev.id"
            class="row"
            role="button"
            tabindex="0"
            @click="goDetail(ev.id)"
            @keydown.enter.prevent="goDetail(ev.id)"
          >
            <td class="titleCell">{{ ev.title }}</td>
            <td>{{ ev.location || '-' }}</td>
            <td>{{ ev.owner?.username || '-' }}</td>
            <td>{{ ev.participants?.length ?? 0 }}</td>
          </tr>
          <tr v-if="filteredEvents.length === 0">
            <td class="muted" colspan="4">
              {{ showMine ? 'Tu neturi savo renginiu.' : 'Kol kas renginiu nera.' }}
            </td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const router = useRouter()

const events = ref([])
const loading = ref(false)
const errorMessage = ref('')
const showMine = ref(false)

const currentUserId = ref(null)
try {
  const raw = localStorage.getItem('user')
  const u = raw ? JSON.parse(raw) : null
  currentUserId.value = u && typeof u.id === 'number' ? u.id : null
} catch {
  currentUserId.value = null
}

const fetchEvents = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    const res = await api.get('/api/events')
    events.value = Array.isArray(res.data) ? res.data : []
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko uzkrauti renginiu.'
    errorMessage.value = String(msg)
    events.value = []
  } finally {
    loading.value = false
  }
}

const isMine = (ev) => {
  if (!currentUserId.value) return false
  const isOwner = ev?.ownerId === currentUserId.value
  const isParticipant = Array.isArray(ev?.participants)
    ? ev.participants.some((p) => p?.userId === currentUserId.value)
    : false
  return isOwner || isParticipant
}

const filteredEvents = computed(() => (showMine.value ? events.value.filter(isMine) : events.value))

const goDetail = (id) => router.push(`/events/${id}`)
const goCreate = () => router.push('/events/new')
const toggleMine = () => {
  if (!currentUserId.value) {
    router.push('/login')
    return
  }
  showMine.value = !showMine.value
}

onMounted(fetchEvents)
</script>

<style scoped>
.events {
  max-width: 980px;
  margin: 0 auto;
  padding: 24px 16px 40px;
}

.header {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 12px;
  margin-bottom: 14px;
}

.actions {
  display: flex;
  gap: 10px;
  align-items: center;
}

.tableWrap {
  overflow: auto;
  border: 1px solid var(--border);
  border-radius: 14px;
  background: var(--surface);
  box-shadow: var(--shadow);
}

.table {
  width: 100%;
  border-collapse: collapse;
  text-align: left;
}

.table th,
.table td {
  padding: 12px 14px;
  border-bottom: 1px solid var(--border);
  white-space: nowrap;
}

.table th {
  font-weight: 650;
  color: var(--text-h);
  background: color-mix(in srgb, var(--surface-2) 75%, transparent);
}

.row {
  cursor: pointer;
}

.row:hover {
  background: var(--accent-bg);
}

.row:focus-visible {
  outline: 2px solid var(--accent);
  outline-offset: -2px;
}

.titleCell {
  font-weight: 650;
  color: var(--text-h);
}

.primary {
  border: 1px solid var(--outline);
  background: linear-gradient(180deg, var(--surface-2), var(--surface));
  color: var(--text-h);
  border-radius: 14px;
  padding: 10px 12px;
  cursor: pointer;
}

.primary:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--outline) 35%, transparent);
}

.secondary {
  border: 1px solid var(--outline-soft);
  background: rgba(255, 255, 255, 0.06);
  color: var(--text-h);
  border-radius: 14px;
  padding: 10px 12px;
  cursor: pointer;
}

.secondary:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--outline) 25%, transparent);
}

.muted {
  color: var(--text);
  opacity: 0.85;
}

.error {
  color: var(--danger);
}
</style>

