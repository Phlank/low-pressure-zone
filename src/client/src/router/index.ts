import { createRouter, createWebHistory } from 'vue-router'
import SchedulesView from '@/components/views/HomeView.vue'
import DashboardView from '@/components/views/DashboardView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'schedules',
      component: SchedulesView
    },
    {
      path: '/dashboard',
      name: 'dashboard',
      component: DashboardView
    }
  ]
})

export default router
