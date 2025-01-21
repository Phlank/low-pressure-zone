import { createRouter, createWebHistory } from 'vue-router'
import SchedulesView from '@/components/views/SchedulesView.vue'
import AboutView from '@/components/views/AboutView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'schedules',
      component: SchedulesView
    },
    {
      path: '/about',
      name: 'about',
      component: AboutView
    }
  ]
})

export default router
