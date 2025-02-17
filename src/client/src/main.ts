import './assets/main.scss'

import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import { PrimeVue } from '@primevue/core'
import { Ripple, ToastService } from 'primevue'
import Aura from '@primevue/themes/aura'
import { createPinia } from 'pinia'

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
