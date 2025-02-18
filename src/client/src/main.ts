import './assets/main.scss'

import { PrimeVue } from '@primevue/core'
import Aura from '@primevue/themes/aura'
import { createPinia } from 'pinia'
import { Ripple, ToastService } from 'primevue'
import { createApp } from 'vue'
import App from './App.vue'
import router from './router'

const pinia = createPinia()
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
app.use(pinia)
app.directive('ripple', Ripple)

app.mount('#app')
