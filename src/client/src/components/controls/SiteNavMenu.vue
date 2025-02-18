<template>
  <div style="display: flex">
    <Button
      icon="pi pi-bars"
      size="large"
      @click="toggleMenu"
      outlined
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
        <a
          v-else
          v-ripple
          :href="item.url"
          :target="item.target"
          v-bind="props.action">
          <span :class="item.icon" />
          <span class="ml-2">{{ item.label }}</span>
        </a>
      </template>
    </Menu>
  </div>
</template>

<script lang="ts" setup>
import { useUserStore } from '@/stores/userStore'
import { Button, Menu } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { computed, onMounted, useTemplateRef } from 'vue'
import { RouterLink } from 'vue-router'

const navMenuRef = useTemplateRef('navMenuRef')
const userStore = useUserStore()

const navMenuItems = computed(() => {
  if (userStore.isLoggedIn()) {
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
    route: '/user/logout'
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
  await userStore.loadIfNotInitialized()
})

const toggleMenu = (event: MouseEvent) => {
  navMenuRef.value?.toggle(event)
}
</script>
