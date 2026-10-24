import './assets/main.scss'

import { PrimeVue } from '@openvue/core'
import Aura from '@openvue/themes/aura'
import { createPinia } from 'pinia'
import { Ripple, ToastService } from 'openvue'
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
