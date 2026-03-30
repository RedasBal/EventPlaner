<template>
  <div class="auth">
    <h2>Prisijungimas</h2>
    <form class="form" @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="El. paštas" required />
      <input v-model="password" type="password" placeholder="Slaptažodis" required />
      <button class="primary" type="submit" :disabled="loading">
        {{ loading ? 'Jungiama...' : 'Prisijungti' }}
      </button>
    </form>
    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p class="muted">
      Neturi paskyros? <router-link class="link" to="/register">Registruotis</router-link>
    </p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const email = ref('')
const password = ref('')
const loading = ref(false)
const errorMessage = ref('')

const router = useRouter()

const handleLogin = async () => {
  errorMessage.value = ''
  loading.value = true

  try {
    const response = await api.post('/api/users/login', {
      email: email.value.trim(),
      password: password.value,
    })

    // Įsimenam prisijungusį vartotoją, kad vėliau galėtume automatiškai priskirti OwnerId.
    // Backend grąžina user objektą (per UserMapper.Map(...)).
    localStorage.setItem('user', JSON.stringify(response.data))

    router.push('/events')
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Prisijungti nepavyko.'
    errorMessage.value = String(msg)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.auth {
  max-width: 520px;
  margin: 0 auto;
  padding: 64px 16px 40px;
  text-align: left;
}

.form {
  display: grid;
  gap: 12px;
  margin-top: 14px;
  padding: 16px;
  border-radius: 16px;
  border: 1px solid var(--outline-soft);
  background: var(--surface);
  box-shadow: var(--shadow);
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

.muted {
  margin-top: 12px;
  color: var(--text);
  opacity: 0.85;
}

.link {
  color: var(--outline);
  text-decoration: none;
}

.link:hover {
  text-decoration: underline;
}

.error {
  margin-top: 12px;
  color: var(--danger);
}
</style>

