import DashboardView from '@/components/views/dashboard/DashboardView.vue'
import DashboardPerformersView from '@/components/views/dashboard/performers/DashboardPerformersView.vue'
import DashboardSchedulesView from '@/components/views/dashboard/schedules/DashboardSchedulesView.vue'
import DashboardUsersView from '@/components/views/dashboard/users/DashboardUsersView.vue'
import HomeView from '@/components/views/home/HomeView.vue'
import LoginView from '@/components/views/user/LoginView.vue'
import RegisterView from '@/components/views/user/RegisterView.vue'
import TwoFactorView from '@/components/views/user/TwoFactorView.vue'
import { allRoles, Role } from '@/constants/roles'
import { useAuthStore } from '@/stores/authStore'
import { createRouter, createWebHistory } from 'vue-router'
import { Routes } from './routes'
import ResendInviteView from '@/components/views/user/ResendInviteView.vue'
import ResetPasswordRequestView from '@/components/views/user/ResetPasswordRequestView.vue'
import ResetPasswordView from '@/components/views/user/ResetPasswordView.vue'
import DashboardCommunitiesView from '@/components/views/dashboard/communities/DashboardCommunitiesView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: HomeView },
    { path: '/user/login', component: LoginView },
    { path: '/user/twoFactor', component: TwoFactorView },
    {
      path: '/user/register',
      component: RegisterView,
      props: (route) => ({ context: route.query.context })
    },
    {
      path: '/user/resendInvite',
      component: ResendInviteView
    },
    {
      path: '/user/resetPassword/request',
      component: ResetPasswordRequestView
    },
    {
      path: '/user/resetPassword',
      component: ResetPasswordView,
      props: (route) => ({ context: route.query.context })
    },
    {
      path: '/dashboard',
      component: DashboardView,
      children: [
        { path: '', name: 'Schedules', component: DashboardSchedulesView },
        { path: 'communities', name: 'Communities', component: DashboardCommunitiesView },
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
    const authStore = useAuthStore()
    await authStore.loadIfNotInitialized()
    if (!authStore.isLoggedIn()) {
      next(Routes.Login)
      return
    }

    const allowedRoles = (to.meta.roles ?? []) as string[]
    if (authStore.isInAnySpecifiedRole(...allowedRoles)) {
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
