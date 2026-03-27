<template>
  <div class="register">
    <h2>Registracija</h2>

    <form @submit.prevent="handleRegister">
      <input v-model="username" type="text" placeholder="Vartotojo vardas" required />
      <input v-model="email" type="email" placeholder="El. paštas" required />
      <input v-model="password" type="password" placeholder="Slaptažodis" required />

      <button type="submit" :disabled="loading">
        {{ loading ? 'Registruojama...' : 'Registruoti' }}
      </button>
    </form>

    <p v-if="errorMessage" class="error">{{ errorMessage }}</p>
    <p>Jau turi paskyrą? <router-link to="/login">Prisijungti</router-link></p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const username = ref('')
const email = ref('')
const password = ref('')

// UI state: leidžia parodyti "Registruojama..." ir užblokuoti mygtuką,
// kol vyksta kreipimasis į backend.
const loading = ref(false)

// Klaidos tekstas, kurį parodome vartotojui, jei backend grąžina error.
const errorMessage = ref('')

const router = useRouter()

const handleRegister = async () => {
  errorMessage.value = ''
  loading.value = true

  try {
    // POST http://localhost:5170/api/users/register
    // Body atitinka backend UserRegisterDto: Username, Email, Password.
    const response = await api.post('/api/users/register', {
      username: username.value.trim(),
      email: email.value.trim(),
      password: password.value,
    })

    // response.data = backend grąžintas user objektas (UserMapper.Map(...)).
    console.log('Registered user:', response.data)

    // Po sėkmingos registracijos nukreipiame į prisijungimą.
    router.push('/login')
  } catch (err) {
    // Jei backend grąžina tekstinę klaidą (pvz. "Neteisingi duomenys"),
    // axios ją turės err.response.data.
    const msg =
      err?.response?.data || err?.message || 'Registracija nepavyko. Bandyk dar kartą.'
    errorMessage.value = String(msg)
  } finally {
    loading.value = false
  }
}
</script>

<style scoped>
.error {
  margin-top: 12px;
  color: #b00020;
}
</style>

