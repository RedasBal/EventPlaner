<template>
  <div class="login">
    <h2>Prisijungimas</h2>
    <form @submit.prevent="handleLogin">
      <input v-model="email" type="email" placeholder="El. paštas" required />
      <input v-model="password" type="password" placeholder="Slaptažodis" required />
      <button type="submit">Prisijungti</button>
    </form>
    <p>Neturi paskyros? <router-link to="/register">Registruotis</router-link></p>
  </div>
</template>

<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import api from '../api/axios.js'

const email = ref('')
const password = ref('')
const router = useRouter()

const handleLogin = async () => {
  const response = await api.post('/api/users/login', {
    email: email.value,
    // Backend'e "Password" yra preferinamas laukas (bet palaikomas ir "passwordHash").
    password: password.value,
  })
  console.log(response.data)
  router.push('/events')
}
</script>

