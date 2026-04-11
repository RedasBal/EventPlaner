import axios from 'axios'

const api = axios.create({
  // Prefer HTTPS (backend launchSettings "https" profile: https://localhost:7064)
  baseURL: import.meta.env.VITE_API_BASE_URL || 'https://localhost:7064',
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('token')
  if (token) {
    config.headers = config.headers || {}
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api
