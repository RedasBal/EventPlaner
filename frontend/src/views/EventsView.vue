<template>
  <div class="events">
    <div class="header">
      <h2>Renginiai</h2>
      <button class="primary" type="button" @click="goCreate">Sukurti savo renginį</button>
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
            v-for="ev in events"
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
          <tr v-if="events.length === 0">
            <td class="muted" colspan="4">Kol kas renginių nėra.</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const router = useRouter()

const events = ref([])
const loading = ref(false)
const errorMessage = ref('')

const fetchEvents = async () => {
  loading.value = true
  errorMessage.value = ''

  try {
    // GET http://localhost:5170/api/events
    const res = await api.get('/api/events')
    events.value = Array.isArray(res.data) ? res.data : []
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko užkrauti renginių.'
    errorMessage.value = String(msg)
    events.value = []
  } finally {
    loading.value = false
  }
}

const goDetail = (id) => router.push(`/events/${id}`)
const goCreate = () => router.push('/events/new')

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

.tableWrap {
  overflow: auto;
  border: 1px solid var(--border);
  border-radius: 10px;
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
  font-weight: 600;
  color: var(--text-h);
  background: var(--social-bg);
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
  font-weight: 600;
  color: var(--text-h);
}

.primary {
  border: 1px solid var(--accent-border);
  background: var(--accent-bg);
  color: var(--text-h);
  border-radius: 10px;
  padding: 10px 12px;
  cursor: pointer;
}

.primary:hover {
  box-shadow: var(--shadow);
}

.muted {
  color: var(--text);
  opacity: 0.85;
}

.error {
  color: #b00020;
}
</style>

