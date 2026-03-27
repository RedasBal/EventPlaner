<template>
  <div class="event-detail">
    <div class="top">
      <router-link class="back" to="/events">← Grįžti</router-link>
    </div>

    <p v-if="loading" class="muted">Kraunama...</p>
    <p v-else-if="errorMessage" class="error">{{ errorMessage }}</p>

    <div v-else class="card">
      <h2 class="title">{{ event?.title }}</h2>
      <p class="meta">
        <span class="pill">Vieta: {{ event?.location || '-' }}</span>
        <span class="pill">Savininkas: {{ event?.owner?.username || '-' }}</span>
        <span class="pill">Dalyviai: {{ event?.participants?.length ?? 0 }}</span>
      </p>

      <p class="desc">{{ event?.description || 'Aprašymo nėra.' }}</p>
    </div>
  </div>
</template>

<script setup>
import { onMounted, ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import api from '../api/axios.js'

const route = useRoute()

const event = ref(null)
const loading = ref(false)
const errorMessage = ref('')

const fetchEvent = async () => {
  const id = Number(route.params.id)
  if (!Number.isFinite(id)) {
    errorMessage.value = 'Neteisingas renginio ID.'
    event.value = null
    return
  }

  loading.value = true
  errorMessage.value = ''

  try {
    // GET http://localhost:5170/api/events/{id}
    const res = await api.get(`/api/events/${id}`)
    event.value = res.data
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko užkrauti renginio.'
    errorMessage.value = String(msg)
    event.value = null
  } finally {
    loading.value = false
  }
}

onMounted(fetchEvent)
watch(() => route.params.id, fetchEvent)
</script>

<style scoped>
.event-detail {
  max-width: 980px;
  margin: 0 auto;
  padding: 24px 16px 40px;
  text-align: left;
}

.top {
  margin-bottom: 14px;
}

.back {
  color: var(--text-h);
  text-decoration: none;
  background: var(--social-bg);
  padding: 8px 10px;
  border-radius: 10px;
  display: inline-flex;
}

.card {
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 18px;
  background: color-mix(in srgb, var(--bg) 92%, var(--accent-bg));
}

.title {
  margin: 0 0 10px;
}

.meta {
  display: flex;
  flex-wrap: wrap;
  gap: 8px;
  margin: 0 0 14px;
}

.pill {
  display: inline-flex;
  padding: 6px 10px;
  border-radius: 999px;
  border: 1px solid var(--border);
  background: var(--social-bg);
}

.desc {
  margin: 0;
}

.muted {
  color: var(--text);
  opacity: 0.85;
}

.error {
  color: #b00020;
}
</style>

