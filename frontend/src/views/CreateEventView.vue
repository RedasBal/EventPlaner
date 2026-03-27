<template>
  <div class="create">
    <div class="top">
      <router-link class="back" to="/events">← Grįžti</router-link>
    </div>

    <h2>Sukurti renginį</h2>

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

      <label class="field">
        <span>Savininko ID (OwnerId)</span>
        <input v-model.number="ownerId" type="number" min="1" required />
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
const ownerId = ref(null)

const loading = ref(false)
const errorMessage = ref('')

onMounted(() => {
  // Jei ateityje login metu išsaugosit user'į, galėsim automatiškai užpildyti OwnerId.
  // Kol kas - tiesiog bandome paimti `user.id` jei jis yra localStorage.
  try {
    const raw = localStorage.getItem('user')
    if (!raw) return
    const u = JSON.parse(raw)
    if (u && typeof u.id === 'number') ownerId.value = u.id
  } catch {
    // ignore
  }
})

const handleCreate = async () => {
  errorMessage.value = ''
  loading.value = true

  try {
    const res = await api.post('/api/events', {
      title: title.value.trim(),
      description: description.value.trim(),
      location: location.value.trim(),
      ownerId: ownerId.value,
    })

    // res.data = sukurtas Event. Nukreipiam į detalę.
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
  background: var(--social-bg);
  padding: 8px 10px;
  border-radius: 10px;
  display: inline-flex;
}

.form {
  margin-top: 16px;
  border: 1px solid var(--border);
  border-radius: 12px;
  padding: 16px;
}

.field {
  display: grid;
  gap: 6px;
  margin-bottom: 12px;
}

.field span {
  color: var(--text-h);
  font-weight: 600;
}

input,
textarea {
  padding: 10px 12px;
  border-radius: 10px;
  border: 1px solid var(--border);
  background: var(--bg);
  color: var(--text-h);
}

.actions {
  display: flex;
  justify-content: flex-end;
  margin-top: 12px;
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

.error {
  margin-top: 12px;
  color: #b00020;
}
</style>

