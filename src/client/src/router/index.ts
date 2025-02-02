import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import SchedulesView from '@/components/views/dashboard/SchedulesView.vue'
import AudiencesView from '@/components/views/dashboard/AudiencesView.vue'
import PerformersView from '@/components/views/dashboard/PerformersView.vue'
import HomeView from '@/components/views/HomeView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      component: HomeView
    },
    {
      path: '/dashboard',
      name: 'Dashboard',
      component: DashboardView,
      children: [
        {
          path: 'schedules',
          name: 'Schedules',
          component: SchedulesView
        },
        {
          path: 'audiences',
          name: 'Audiences',
          component: AudiencesView
        },
        {
          path: 'performers',
          name: 'Performers',
          component: PerformersView
        }
      ]
    }
  ]
})

export default router
