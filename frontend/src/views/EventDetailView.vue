<template>
  <div class="event-detail">
    <h2>{{ event.title }}</h2>
    <p>{{ event.description }}</p>
    <p>📅 {{ event.date }}</p>
    <router-link to="/events">← Grįžti</router-link>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const event = ref({})
</script>