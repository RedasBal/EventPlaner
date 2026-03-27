<template>
  <div class="events">
    <h2>Renginiai</h2>
    <div v-for="event in events" :key="event.id" class="event-card">
      <h3>{{ event.title }}</h3>
      <p>{{ event.date }}</p>
      <router-link :to="`/events/${event.id}`">Plačiau</router-link>
    </div>
  </div>
</template>

<script setup>
import { ref } from 'vue'

const events = ref([])
</script>