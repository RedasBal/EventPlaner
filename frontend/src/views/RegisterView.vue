<template>
  <div class="register">
    <h2>Registracija</h2>
    <form @submit.prevent="handleRegister">
      <input v-model="username" type="text" placeholder="Vartotojo vardas" required />
      <input v-model="email" type="email" placeholder="El. paštas" required />
      <input v-model="password" type="password" placeholder="Slaptažodis" required />
      <button type="submit">Registruotis</button>
    </form>
    <p>Jau turi paskyrą? <router-link to="/login">Prisijungti</router-link></p>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const username = ref('')
const email = ref('')
const password = ref('')

const handleRegister = () => {
  console.log('Register:', username.value, email.value)
}
</script>