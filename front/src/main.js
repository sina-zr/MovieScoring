// main.js

import { createApp } from 'vue'
import './assets/main.css'
import Toast from "vue-toastification";
import "vue-toastification/dist/index.css";
import 'primeicons/primeicons.css'
import router from './router'
import App from './App.vue'
import PrimeVue from 'primevue/config';

const app = createApp(App)

app.use(router)

app.use(PrimeVue);
app.use(Toast, {
    transition: "Vue-Toastification__bounce",
    maxToasts: 20,
    newestOnTop: true
});

app.mount('#app')
