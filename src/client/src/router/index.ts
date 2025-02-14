import { createRouter, createWebHistory } from 'vue-router'
import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import DashboardSchedulesView from '@/components/views/dashboard/schedules/DashboardSchedulesView.vue'
import DashboardAudiencesView from '@/components/views/dashboard/DashboardAudiencesView.vue'
import DashboardPerformersView from '@/components/views/dashboard/DashboardPerformersView.vue'
import HomeView from '@/components/views/HomeView.vue'
import LoginView from '@/components/views/LoginView.vue'
import RegisterView from '@/components/views/RegisterView.vue'
import { getAuth, onAuthStateChanged } from 'firebase/auth'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView },
    { path: '/user/login', component: LoginView },
    { path: '/user/register', component: RegisterView },
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

const getCurrentUser = () => {
  return new Promise((resolve, reject) => {
    const removeListener = onAuthStateChanged(
      getAuth(),
      (user) => {
        removeListener()
        resolve(user)
      },
      reject
    )
  })
}

router.beforeEach(async (to, from, next) => {
  if (to.matched.some((record) => record.meta.requiresAuthentication)) {
    if (await getCurrentUser()) {
      next()
    } else {
      next('/')
    }
  } else {
    next()
  }
})

export default router
