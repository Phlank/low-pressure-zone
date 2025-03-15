<template>
  <div style="display: flex">
    <Button
      icon="pi pi-bars"
      size="large"
      @click="toggleMenu"
      outlined
      severity="secondary"
      aria-controls="navigation-menu" />
    <Menu
      id="navigation-menu"
      ref="navMenuRef"
      :model="navMenuItems"
      :popup="true">
      <!-- https://primevue.org/menu/#router -->
      <template #item="{ item, props }">
        <RouterLink
          v-if="item.route"
          v-slot="{ href, navigate }"
          :to="item.route"
          custom>
          <a
            v-ripple
            :href="href"
            v-bind="props.action"
            @click="navigate">
            <span :class="item.icon" />
            <span class="ml-2">{{ item.label }}</span>
          </a>
        </RouterLink>
        <div v-else>
          <a
            v-ripple
            class="p-menu-item-link"
            @click="item.callback">
            <span :class="item.icon" />
            <span class="ml-2">{{ item.label }}</span>
          </a>
        </div>
      </template>
    </Menu>
  </div>
</template>

<script lang="ts" setup>
import router from '@/router'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { Button, Menu } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { computed, onMounted, useTemplateRef } from 'vue'
import { RouterLink } from 'vue-router'
import authApi from '@/api/resources/authApi.ts'

const navMenuRef = useTemplateRef('navMenuRef')
const authStore = useAuthStore()

const navMenuItems = computed(() => {
  if (authStore.isLoggedIn()) {
    return loggedInNavMenuItems
  } else {
    return loggedOutNavMenuItems
  }
})

const loggedInNavMenuItems: MenuItem[] = [
  {
    label: 'Dashboard',
    labelString: 'Dashboard',
    icon: 'pi pi-cog',
    route: '/dashboard'
  },
  {
    label: 'Logout',
    labelString: 'Logout',
    icon: 'pi pi-sign-out',
    callback: async () => {
      await authStore.loadIfNotInitialized()
      if (authStore.isLoggedIn()) {
        await authApi.getLogout()
      }
      await authStore.load()
      await router.push(Routes.Home)
    }
  }
]

const loggedOutNavMenuItems: MenuItem[] = [
  {
    label: 'Login',
    labelString: 'Login',
    icon: 'pi pi-sign-in',
    route: '/user/login'
  }
]

onMounted(async () => {
  await authStore.loadIfNotInitialized()
})

const toggleMenu = (event: MouseEvent) => {
  navMenuRef.value?.toggle(event)
}
</script>
