import './assets/main.scss'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { PrimeVue } from '@primevue/core'
import { ToastService } from 'primevue'
import Aura from '@primevue/themes/aura'

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
  }
})
app.use(ToastService)

app.mount('#app')
