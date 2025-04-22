// router/index.js

import AddMovieView from '@/Views/AddMovieView.vue'
import HomeView from '@/Views/HomeView.vue'
import MovieDetailView from '@/Views/MovieDetailView.vue'
import LoginView from '@/Views/LoginView.vue'
import MovieView from '@/Views/MovieView.vue'
import NotFound from '@/Views/NotFound.vue'
import { createRouter, createWebHistory } from 'vue-router'
import { isAuthenticated } from '@/services/auth'
import RegisterView from '@/Views/RegisterView.vue'
import WatchlistView from '@/Views/WatchlistView.vue'

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        { path: '/login', name: 'Login', component: LoginView },
        { path: '/register', name: 'Register', component: RegisterView },
        { path: '/', name: 'Home', component: HomeView },
        { path: '/movies', name: 'Movies', component: MovieView },
        { path: '/movies/:id', name: 'Movie Detail', component: MovieDetailView },
        { path: '/movies/add', name: 'Add Movies', component: AddMovieView, meta: { requiresAuth: true } },
        { path: '/watchlist', name: 'Watchlist', component: WatchlistView, meta: { requiresAuth: true } },
        { path: '/:catchAll(.*)', name: 'Not Found', component: NotFound }
    ]
})


router.beforeEach((to, from, next) => {
    if (to.meta.requiresAuth && !isAuthenticated.value) {
        return next({ name: 'Login' })
    }
    // prevent logged‑in users from seeing Login page:
    if (to.name === 'Login' && isAuthenticated.value) {
        return next({ name: 'Home' })
    }
    next()
})


export default router