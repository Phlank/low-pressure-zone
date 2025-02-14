import './assets/main.scss'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { PrimeVue } from '@primevue/core'
import { Ripple, ToastService } from 'primevue'
import Aura from '@primevue/themes/aura'
// Import the functions you need from the SDKs you need
import { initializeApp } from 'firebase/app'
// TODO: Add SDKs for Firebase products that you want to use
// https://firebase.google.com/docs/web/setup#available-libraries

// Your web app's Firebase configuration
const firebaseConfig = {
  apiKey: import.meta.env.VITE_FIREBASE_API_KEY,
  authDomain: import.meta.env.VITE_FIREBASE_AUTH_DOMAIN,
  projectId: import.meta.env.VITE_FIREBASE_PROJECT_ID,
  storageBucket: import.meta.env.VITE_FIREBASE_STORAGE_BUCKET,
  messagingSenderId: import.meta.env.VITE_FIREBASE_MESSAGING_SENDER_ID,
  appId: import.meta.env.VITE_FIREBASE_APP_ID
}
initializeApp(firebaseConfig)

const app = createApp(App)
app.use(router)
app.use(PrimeVue, {
  theme: {
    preset: Aura,
    options: {
      prefix: 'p',
      darkModeSelector: '.dark-mode-toggle',
      cssLayer: false
    }
  },
  ripple: true
})
app.use(ToastService)
app.directive('ripple', Ripple)

app.mount('#app')
