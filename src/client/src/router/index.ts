import { allRoles, Role } from '@/constants/roles'
import { useAuthStore } from '@/stores/authStore'
import { createRouter, createWebHistory } from 'vue-router'
import { Routes } from './routes'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    { path: '/', component: () => import('@/components/views/home/HomeView.vue') },
    { path: '/user/login', component: () => import('@/components/views/user/LoginView.vue') },
    {
      path: '/user/twoFactor',
      component: () => import('@/components/views/user/TwoFactorView.vue')
    },
    {
      path: '/user/register',
      component: () => import('@/components/views/user/RegisterView.vue'),
      props: (route) => ({ context: route.query.context })
    },
    {
      path: '/user/resendInvite',
      component: () => import('@/components/views/user/ResendInviteView.vue')
    },
    {
      path: '/user/resetPassword/request',
      component: () => import('@/components/views/user/ResetPasswordRequestView.vue')
    },
    {
      path: '/user/resetPassword',
      component: () => import('@/components/views/user/ResetPasswordView.vue'),
      props: (route) => ({ context: route.query.context })
    },
    {
      path: '/dashboard',
      component: () => import('@/components/views/dashboard/DashboardView.vue'),
      children: [
        {
          path: '',
          name: 'Schedules',
          component: () =>
            import('@/components/views/dashboard/schedules/DashboardSchedulesView.vue')
        },
        {
          path: 'communities',
          name: 'Communities',
          component: () =>
            import('@/components/views/dashboard/communities/DashboardCommunitiesView.vue')
        },
        {
          path: 'performers',
          name: 'Performers',
          component: () =>
            import('@/components/views/dashboard/performers/DashboardPerformersView.vue')
        },
        {
          path: 'users',
          name: 'Users',
          component: () => import('@/components/views/dashboard/users/DashboardUsersView.vue'),
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
