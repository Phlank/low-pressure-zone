import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import DashboardSchedulesView from '@/components/views/dashboard/DashboardSchedulesView.vue'
import DashboardAudiencesView from '@/components/views/dashboard/DashboardAudiencesView.vue'
import DashboardPerformersView from '@/components/views/dashboard/performers/DashboardPerformersView.vue'
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
      component: DashboardView,
      children: [
        {
          path: '',
          name: 'Schedules',
          component: DashboardSchedulesView
        },
        {
          path: 'audiences',
          name: 'Audiences',
          component: DashboardAudiencesView
        },
        {
          path: 'performers',
          name: 'Performers',
          component: DashboardPerformersView
        }
      ]
    }
  ]
})

export default router
