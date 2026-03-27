import axios from 'axios'

const api = axios.create({
    baseURL: 'https://localhost:5170', // <-- pakeisk į savo portą
})

export default api