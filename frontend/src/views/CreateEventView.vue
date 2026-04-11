<template>
  <div class="create">
    <div class="top">
      <router-link class="back" to="/events">← Grįžti</router-link>
    </div>

    <h2>Sukurti renginį</h2>
    <p class="muted">Renginys bus priskirtas prisijungusiam vartotojui.</p>

    <form class="form" @submit.prevent="handleCreate">
      <label class="field">
        <span>Pavadinimas</span>
        <input v-model="title" type="text" required />
      </label>

      <label class="field">
        <span>Vieta</span>
        <input v-model="location" type="text" />
      </label>

      <label class="field">
        <span>Aprašymas</span>
        <textarea v-model="description" rows="5" />
      </label>

      <div class="actions">
        <button class="primary" type="submit" :disabled="loading">
          {{ loading ? 'Kuriama...' : 'Sukurti' }}
        </button>
      </div>

      <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    </form>
  </div>
</template>

<script setup>
import { onMounted, ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const router = useRouter()

const title = ref('')
const description = ref('')
const location = ref('')

const loading = ref(false)
const errorMessage = ref('')

onMounted(() => {
  const token = localStorage.getItem('token')
  if (!token) {
    errorMessage.value = 'Norint sukurti renginį, reikia prisijungti.'
    router.push('/login')
  }
})

const handleCreate = async () => {
  errorMessage.value = ''

  const token = localStorage.getItem('token')
  if (!token) {
    errorMessage.value = 'Prisijunkite ir bandykite dar kartą.'
    router.push('/login')
    return
  }

  loading.value = true

  try {
    // POST http://localhost:5170/api/events
    // Body atitinka backend CreateEventDto: Title, Description, Location, OwnerId.
    const res = await api.post('/api/events', {
      title: title.value.trim(),
      description: description.value.trim(),
      location: location.value.trim(),
    })

    router.push(`/events/${res.data?.id ?? ''}`)
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko sukurti renginio.'
    errorMessage.value = String(msg)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.create {
  max-width: 720px;
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
  background: var(--surface-2);
  padding: 8px 10px;
  border-radius: 12px;
  display: inline-flex;
  border: 1px solid var(--outline-soft);
}

.muted {
  color: var(--text);
  opacity: 0.85;
  margin: 0 0 10px;
}

.form {
  margin-top: 14px;
  border: 1px solid var(--outline-soft);
  border-radius: 16px;
  padding: 16px;
  background: var(--surface);
  box-shadow: var(--shadow);
}

.field {
  display: grid;
  gap: 6px;
  margin-bottom: 12px;
}

.field span {
  color: var(--text-h);
  font-weight: 650;
}

.actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
}

.primary {
  border: 1px solid var(--outline);
  background: linear-gradient(180deg, var(--surface-2), var(--surface));
  color: var(--text-h);
  border-radius: 14px;
  padding: 10px 14px;
  cursor: pointer;
}

.primary:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--outline) 35%, transparent);
}

.error {
  margin-top: 12px;
  color: var(--danger);
}
</style>
