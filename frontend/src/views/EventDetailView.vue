<template>
  <div class="event-detail">
    <div class="top">
      <router-link class="back" to="/events">← Grizti</router-link>

      <div class="topActions" v-if="event">
        <button
          v-if="canJoin"
          class="primary"
          type="button"
          :disabled="actionLoading"
          @click="joinEvent"
        >
          {{ actionLoading ? 'Vykdoma...' : 'Prisijungti' }}
        </button>

        <button
          v-if="canLeave"
          class="secondary"
          type="button"
          :disabled="actionLoading"
          @click="leaveEvent"
        >
          {{ actionLoading ? 'Vykdoma...' : 'Palikti' }}
        </button>

        <button
          v-if="canDelete"
          class="danger"
          type="button"
          :disabled="actionLoading"
          @click="deleteEvent"
        >
          {{ actionLoading ? 'Vykdoma...' : 'Istrinti rengini' }}
        </button>
      </div>
    </div>

    <p v-if="loading" class="muted">Kraunama...</p>
    <p v-else-if="errorMessage" class="error">{{ errorMessage }}</p>

    <div v-else-if="event" class="card">
      <h2 class="title">{{ event.title }}</h2>
      <p class="meta">
        <span class="pill">Vieta: {{ event.location || '-' }}</span>
        <span class="pill">Savininkas: {{ event.owner?.username || '-' }}</span>
        <span class="pill">Dalyviai: {{ event.participants?.length ?? 0 }}</span>
      </p>

      <p class="desc">{{ event.description || 'Aprasymo nera.' }}</p>

      <p v-if="currentUserId && isOwner" class="note">Tu esi sio renginio savininkas.</p>
      <p v-else-if="currentUserId && isParticipant" class="note">Tu jau dalyvauji siame renginyje.</p>
      <p v-else-if="!currentUserId" class="note">
        Prisijunk, kad galetum prisijungti prie renginio.
      </p>
    </div>

    <div v-if="event" class="card chatCard">
      <div class="chatHeader">
        <h3 class="chatTitle">Komentarai</h3>
        <button
          class="secondary smallBtn"
          type="button"
          :disabled="commentsLoading || !canUseChat"
          @click="fetchComments"
        >
          {{ commentsLoading ? 'Kraunama...' : 'Atnaujinti' }}
        </button>
      </div>

      <p v-if="!currentUserId" class="muted">
        Prisijunk, kad matytum ir galetum rasyti komentarus.
      </p>
      <p v-else-if="!canUseChat" class="muted">
        Prisijunk prie renginio, kad matytum ir galetum rasyti komentarus.
      </p>

      <p v-else-if="commentsError" class="error">{{ commentsError }}</p>

      <div v-else class="chatBody">
        <div class="messages" ref="messagesEl">
          <p v-if="comments.length === 0" class="muted">Komentaru dar nera.</p>

          <div v-for="c in comments" :key="c.id" class="msg">
            <div class="msgTop">
              <span class="msgUser">
                {{ c.userId === currentUserId ? 'Tu' : c.username || 'Nezinomas' }}
              </span>
              <button
                v-if="canDeleteComment(c)"
                class="ghostDanger"
                type="button"
                :disabled="sendLoading"
                @click="deleteComment(c.id)"
              >
                Trinti
              </button>
            </div>
            <div class="msgText">{{ c.text }}</div>
          </div>
        </div>

        <form class="composer" @submit.prevent="sendComment">
          <textarea
            v-model="newComment"
            rows="3"
            placeholder="Parasyk komentara..."
            :disabled="sendLoading"
          />
          <div class="composerActions">
            <button class="primary" type="submit" :disabled="sendDisabled">
              {{ sendLoading ? 'Siunciama...' : 'Siusti' }}
            </button>
          </div>
        </form>
      </div>
    </div>
  </div>
</template>

<script setup>
import { computed, onMounted, onUnmounted, ref, watch } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../api/axios.js'

const route = useRoute()
const router = useRouter()

const event = ref(null)
const loading = ref(false)
const errorMessage = ref('')
const actionLoading = ref(false)

const comments = ref([])
const commentsLoading = ref(false)
const commentsError = ref('')
const newComment = ref('')
const sendLoading = ref(false)
const messagesEl = ref(null)
let pollHandle = null

const currentUserId = ref(null)
try {
  const raw = localStorage.getItem('user')
  const u = raw ? JSON.parse(raw) : null
  currentUserId.value = u && typeof u.id === 'number' ? u.id : null
} catch {
  currentUserId.value = null
}

const eventId = computed(() => Number(route.params.id))

const isOwner = computed(
  () => !!(event.value && currentUserId.value && event.value.ownerId === currentUserId.value),
)
const isParticipant = computed(() => {
  if (!event.value || !currentUserId.value) return false
  return Array.isArray(event.value.participants)
    ? event.value.participants.some((p) => p?.userId === currentUserId.value)
    : false
})

const canUseChat = computed(() => !!(currentUserId.value && (isOwner.value || isParticipant.value)))

const canJoin = computed(
  () => !!(currentUserId.value && event.value && !isOwner.value && !isParticipant.value),
)
const canLeave = computed(
  () => !!(currentUserId.value && event.value && !isOwner.value && isParticipant.value),
)
const canDelete = computed(() => !!(currentUserId.value && event.value && isOwner.value))

const sendDisabled = computed(() => {
  if (!canUseChat.value) return true
  if (sendLoading.value) return true
  return !newComment.value || !newComment.value.trim()
})

const fetchEvent = async () => {
  if (!Number.isFinite(eventId.value)) {
    errorMessage.value = 'Neteisingas renginio ID.'
    event.value = null
    return
  }

  loading.value = true
  errorMessage.value = ''
  // Avoid showing stale data when navigating between events.
  event.value = null
  comments.value = []
  commentsError.value = ''

  try {
    const res = await api.get(`/api/events/${eventId.value}`)
    event.value = res.data
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko uzkrauti renginio.'
    errorMessage.value = String(msg)
    event.value = null
  } finally {
    loading.value = false
  }
}

const fetchComments = async () => {
  if (!canUseChat.value) {
    comments.value = []
    commentsError.value = ''
    return
  }

  commentsLoading.value = true
  commentsError.value = ''
  try {
    const res = await api.get(`/api/events/${eventId.value}/comments`, {
      params: { userId: currentUserId.value },
    })
    comments.value = Array.isArray(res.data) ? res.data : []

    requestAnimationFrame(() => {
      if (messagesEl.value) messagesEl.value.scrollTop = messagesEl.value.scrollHeight
    })
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko uzkrauti komentaru.'
    commentsError.value = String(msg)
    comments.value = []
  } finally {
    commentsLoading.value = false
  }
}

const sendComment = async () => {
  if (sendDisabled.value) return

  const text = newComment.value.trim()
  sendLoading.value = true
  commentsError.value = ''
  try {
    await api.post(
      `/api/events/${eventId.value}/comments`,
      { text },
      { params: { userId: currentUserId.value } },
    )
    newComment.value = ''
    await fetchComments()
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko issiusti komentaro.'
    commentsError.value = String(msg)
  } finally {
    sendLoading.value = false
  }
}

const canDeleteComment = (c) => {
  if (!currentUserId.value) return false
  return c?.userId === currentUserId.value || isOwner.value
}

const deleteComment = async (commentId) => {
  if (!canUseChat.value) return
  if (!confirm('Trinti komentara?')) return

  sendLoading.value = true
  commentsError.value = ''
  try {
    await api.delete(`/api/events/${eventId.value}/comments/${commentId}`, {
      params: { userId: currentUserId.value },
    })
    await fetchComments()
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko istrinti komentaro.'
    commentsError.value = String(msg)
  } finally {
    sendLoading.value = false
  }
}

const joinEvent = async () => {
  if (!canJoin.value) return
  actionLoading.value = true
  try {
    await api.post(`/api/events/${eventId.value}/join`, null, {
      params: { userId: currentUserId.value },
    })
    await fetchEvent()
    await fetchComments()
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko prisijungti.'
    errorMessage.value = String(msg)
  } finally {
    actionLoading.value = false
  }
}

const leaveEvent = async () => {
  if (!canLeave.value) return
  actionLoading.value = true
  try {
    await api.post(`/api/events/${eventId.value}/leave`, null, {
      params: { userId: currentUserId.value },
    })
    await fetchEvent()
    comments.value = []
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko palikti renginio.'
    errorMessage.value = String(msg)
  } finally {
    actionLoading.value = false
  }
}

const deleteEvent = async () => {
  if (!canDelete.value) return
  if (!confirm('Tikrai istrinti rengini?')) return
  actionLoading.value = true
  try {
    await api.delete(`/api/events/${eventId.value}`, {
      params: { userId: currentUserId.value },
    })
    router.push('/events')
  } catch (err) {
    const msg = err?.response?.data || err?.message || 'Nepavyko istrinti renginio.'
    errorMessage.value = String(msg)
  } finally {
    actionLoading.value = false
  }
}

onMounted(fetchEvent)
watch(() => route.params.id, fetchEvent)

watch(
  canUseChat,
  (ok) => {
    if (pollHandle) {
      clearInterval(pollHandle)
      pollHandle = null
    }
    if (!ok) {
      comments.value = []
      return
    }

    fetchComments()
    pollHandle = setInterval(fetchComments, 5000)
  },
  { immediate: true },
)

onUnmounted(() => {
  if (pollHandle) clearInterval(pollHandle)
})
</script>

<style scoped>
.event-detail {
  max-width: 980px;
  margin: 0 auto;
  padding: 24px 16px 40px;
  text-align: left;
}

.top {
  display: flex;
  justify-content: space-between;
  gap: 10px;
  align-items: center;
  margin-bottom: 14px;
  flex-wrap: wrap;
}

.topActions {
  display: flex;
  gap: 10px;
  flex-wrap: wrap;
}

.back {
  color: var(--text-h);
  text-decoration: none;
  background: rgba(255, 255, 255, 0.06);
  padding: 8px 10px;
  border-radius: 12px;
  display: inline-flex;
  border: 1px solid var(--outline-soft);
}

.card {
  border: 1px solid var(--border);
  border-radius: 16px;
  padding: 18px;
  background: var(--surface);
  box-shadow: var(--shadow);
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
  background: rgba(255, 255, 255, 0.06);
}

.desc {
  margin: 0;
}

.note {
  margin-top: 14px;
  color: var(--text);
  opacity: 0.9;
}

.primary,
.secondary,
.danger {
  border-radius: 14px;
  padding: 10px 12px;
  cursor: pointer;
}

.primary {
  border: 1px solid var(--outline);
  background: linear-gradient(180deg, var(--surface-2), var(--surface));
  color: var(--text-h);
}

.primary:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--outline) 35%, transparent);
}

.secondary {
  border: 1px solid var(--outline-soft);
  background: rgba(255, 255, 255, 0.06);
  color: var(--text-h);
}

.secondary:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--outline) 25%, transparent);
}

.danger {
  border: 1px solid color-mix(in srgb, var(--danger) 70%, transparent);
  background: rgba(255, 77, 125, 0.12);
  color: var(--text-h);
}

.danger:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--danger) 25%, transparent);
}

.muted {
  color: var(--text);
  opacity: 0.85;
}

.error {
  color: var(--danger);
}

.chatCard {
  margin-top: 14px;
  padding: 16px;
}

.chatHeader {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 10px;
  margin-bottom: 10px;
}

.chatTitle {
  margin: 0;
}

.chatBody {
  display: grid;
  gap: 12px;
}

.messages {
  border: 1px solid var(--border);
  border-radius: 14px;
  background: rgba(0, 0, 0, 0.18);
  padding: 12px;
  max-height: 320px;
  overflow: auto;
}

.msg {
  padding: 10px 10px;
  border-radius: 12px;
  border: 1px solid rgba(255, 255, 255, 0.06);
  background: rgba(255, 255, 255, 0.04);
  margin-bottom: 10px;
}

.msgTop {
  display: flex;
  align-items: baseline;
  justify-content: space-between;
  gap: 10px;
  margin-bottom: 6px;
}

.msgUser {
  font-weight: 650;
  color: var(--text-h);
}

.msgText {
  white-space: pre-wrap;
  word-break: break-word;
}

.composerActions {
  display: flex;
  justify-content: flex-end;
}

.ghostDanger {
  border: 1px solid rgba(255, 77, 125, 0.45);
  background: rgba(255, 77, 125, 0.08);
  color: var(--text-h);
  border-radius: 12px;
  padding: 6px 10px;
  cursor: pointer;
}

.ghostDanger:hover {
  box-shadow: 0 0 0 3px color-mix(in srgb, var(--danger) 20%, transparent);
}

.smallBtn {
  padding: 8px 10px;
  border-radius: 12px;
}
</style>
