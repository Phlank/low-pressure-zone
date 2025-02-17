import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import DashboardSchedulesView from '@/components/views/dashboard/schedules/DashboardSchedulesView.vue'
import DashboardAudiencesView from '@/components/views/dashboard/DashboardAudiencesView.vue'
import DashboardPerformersView from '@/components/views/dashboard/DashboardPerformersView.vue'
import HomeView from '@/components/views/HomeView.vue'
import LoginView from '@/components/views/users/LoginView.vue'
import RegisterView from '@/components/views/users/RegisterView.vue'
import { getAuth, onAuthStateChanged } from 'firebase/auth'
import TwoFactorView from '@/components/views/users/TwoFactorView.vue'
import api from '@/api/api'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView },
    { path: '/user/login', component: LoginView },
    { path: '/user/register', component: RegisterView },
    { path: '/user/twofactor', component: TwoFactorView },
    {
      path: '/dashboard',
      component: DashboardView,
      children: [
        { path: '', name: 'Schedules', component: DashboardSchedulesView },
        { path: 'audiences', name: 'Audiences', component: DashboardAudiencesView },
        { path: 'performers', name: 'Performers', component: DashboardPerformersView }
      ],
      meta: {
        requiresAuthentication: true
      }
    }
  ]
})

const getCurrentUser = async () => {
  const response = await api.users.info.get()
  console.log(JSON.stringify(response))
  if (!response.isSuccess()) return undefined

  return response.data
}

router.beforeEach(async (to, from, next) => {
  if (to.matched.some((record) => record.meta.requiresAuthentication)) {
    const user = await getCurrentUser()
    if (!user) {
      next('/')
      return
    }
    next()
  } else {
    next()
  }
})

export default router
export const LOGIN_REDIRECT = '/dashboard'
