import DashboardAudiencesView from '@/components/views/dashboard/DashboardAudiencesView.vue'
import DashboardPerformersView from '@/components/views/dashboard/DashboardPerformersView.vue'
import DashboardUsersView from '@/components/views/dashboard/users/DashboardUsersView.vue'
import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import DashboardSchedulesView from '@/components/views/dashboard/schedules/DashboardSchedulesView.vue'
import HomeView from '@/components/views/HomeView.vue'
import LoginView from '@/components/views/user/LoginView.vue'
import LogoutView from '@/components/views/user/LogoutView.vue'
import TwoFactorView from '@/components/views/user/TwoFactorView.vue'
import { allRoles, Role } from '@/constants/roles'
import { useUserStore } from '@/stores/userStore'
import { createRouter, createWebHistory } from 'vue-router'
import { Routes } from './routes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView },
    { path: '/user/login', component: LoginView },
    { path: '/user/logout', component: LogoutView },
    { path: '/user/twofactor', component: TwoFactorView },
    {
      path: '/dashboard',
      component: DashboardView,
      children: [
        { path: '', name: 'Schedules', component: DashboardSchedulesView },
        { path: 'audiences', name: 'Audiences', component: DashboardAudiencesView },
        { path: 'performers', name: 'Performers', component: DashboardPerformersView },
        {
          path: 'users',
          name: 'Users',
          component: DashboardUsersView,
          meta: { auth: true, roles: [Role.Admin] }
        }
      ],
      meta: {
        auth: true,
        roles: allRoles
      }
    }
  ]
})

router.beforeEach(async (to, from, next) => {
  if (to.meta.auth) {
    const userStore = useUserStore()
    await userStore.loadIfNotInitialized()
    if (!userStore.isLoggedIn()) {
      next(Routes.Login)
      return
    }

    const allowedRoles = (to.meta.roles ?? []) as string[]
    if (userStore.isInAnySpecifiedRole(...allowedRoles)) {
      next()
      return
    }

    next(Routes.Login)
  } else {
    next()
  }
})

export default router
export const defaultLoginRedirect = Routes.Schedules
