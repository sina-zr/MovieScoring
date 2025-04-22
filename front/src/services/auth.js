// src/services/auth.js
import axios from 'axios'
import { ref, computed } from 'vue'
import router from '@/router'
import { useToast } from 'vue-toastification'

// -- 1) Reactive token + bootstrap default header on page load
const rawToken = localStorage.getItem('jwt_token') || ''
const token = ref(rawToken)
const isAuthenticated = ref(!!token.value)
const isAdmin = computed(() => {
    if (!token.value) return false
    const payload = parseJwtPayload(token.value)
    return payload?.['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'] === 'Admin'
})
const toast = useToast()

// -- 2) Create your “authApi” instance
const authApi = axios.create({
    baseURL: 'https://localhost:7214',
    headers: {
        'Content-Type': 'application/json'
    }
})

// -- 3) Interceptor to inject latest token on each request
authApi.interceptors.request.use(config => {
    const t = localStorage.getItem('jwt_token')
    if (t) {
        config.headers.Authorization = `Bearer ${t}`
    }
    return config
})

// Utility to decode JWT payload
function parseJwtPayload(jwt) {
    try {
        const base64Url = jwt.split('.')[1]
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/')
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split('')
                .map(c => '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2))
                .join('')
        )
        return JSON.parse(jsonPayload)
    } catch {
        return null
    }
}

// -- 4) Export your composable
export function useAuth() {
    const authError = ref(null)

    // a) login via the “public” axios (or you could use authApi here too)
    async function login(username, password) {
        authError.value = null
        try {
            const { data } = await axios.post(
                `https://localhost:7214/api/login`,
                { username, password }
            )
            token.value = data
            localStorage.setItem('jwt_token', data)
            isAuthenticated.value = true
            router.push({ name: 'Home' })
        }
        catch (err) {
            authError.value = err.response?.data || err.message
        }
    }

    async function register(username, password, fullName) {
        authError.value = null
        try {
            const res = await axios.post(
                `https://localhost:7214/api/register`,
                { username, password, fullName }
            )
            if (res.status != 200) {
                console.log(res.status + res.data)
                toast.error('Faild to Register')
            }
            console.log(res.data)
            token.value = ''
            localStorage.removeItem('jwt_token')
            isAuthenticated.value = false
            toast.success('Successfully Registered, now Log In')
            router.push({ name: 'Login' })
        }
        catch (err) {
            authError.value = err.response?.data || err.message
        }
    }

    // b) clear out when logging out
    function logout() {
        token.value = ''
        localStorage.removeItem('jwt_token')
        isAuthenticated.value = false
        router.push({ name: 'Login' })
    }

    return { login, logout, authError, register }
}

// -- 5) Export the authApi for other modules to use
export { authApi, isAuthenticated, isAdmin }
