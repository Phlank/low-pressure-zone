<template>
  <div style="display: flex">
    <Button
      aria-controls="navigation-menu"
      icon="pi pi-bars"
      outlined
      severity="secondary"
      size="large"
      @click="toggleMenu" />
    <Menu
      id="navigation-menu"
      ref="navMenuRef"
      :model="navMenuItems"
      :popup="true">
      <!-- https://primevue.org/menu/#router -->
      <template #item="{ item, props }">
        <div>
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
              <span
                v-if="item.icon"
                :class="item.icon" />
              <img
                v-if="item.iconSvg"
                :src="item.iconSvg()"
                style="max-width: 16px; max-height: 16px" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </RouterLink>
          <div v-else-if="item.href">
            <a
              v-ripple
              :href="item.href"
              class="p-menu-item-link">
              <span
                v-if="item.icon"
                :class="item.icon" />
              <img
                v-if="item.iconSvg"
                :src="item.iconSvg()"
                style="max-width: 16px; max-height: 16px" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </div>
          <div v-else>
            <a
              v-ripple
              class="p-menu-item-link"
              @click="item.callback">
              <span
                v-if="item.icon"
                :class="item.icon" />
              <img
                v-if="item.iconSvg"
                :src="item.iconSvg()"
                style="max-width: 16px; max-height: 16px" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </div>
        </div>
      </template>
    </Menu>
  </div>
</template>

<script lang="ts" setup>
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { Button, Menu } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { computed, inject, onMounted, ref, type Ref, useTemplateRef } from 'vue'
import { RouterLink, useRouter } from 'vue-router'
import authApi from '@/api/resources/authApi.ts'

const router = useRouter()
const navMenuRef = useTemplateRef('navMenuRef')
const authStore = useAuthStore()
const discordInvite = import.meta.env.VITE_DISCORD_INVITE_LINK
const githubUrl = import.meta.env.VITE_GITHUB_URL
const soundcloudUrl = import.meta.env.VITE_SOUNDCLOUD_URL
const isDarkMode: Ref<boolean> = inject('isDarkMode', ref(true))

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
    route: '/dashboard',
    visible: true
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
    },
    visible: true
  },
  {
    label: 'Github',
    labelString: 'Github',
    icon: 'pi pi-github',
    href: githubUrl,
    visible: true
  },
  {
    label: 'Chat',
    labelString: 'Chat',
    icon: 'pi pi-discord',
    href: discordInvite,
    visible: () => window.innerWidth < 240
  },
  {
    label: 'Soundcloud',
    labelString: 'Soundcloud',
    href: soundcloudUrl,
    iconSvg: () => (isDarkMode.value ? '/soundcloud-logo-white.svg' : '/soundcloud-logo-black.svg'),
    visible: true
  }
]

const loggedOutNavMenuItems: MenuItem[] = [
  {
    label: 'Login',
    labelString: 'Login',
    icon: 'pi pi-sign-in',
    route: '/user/login',
    visible: true
  },
  {
    label: 'Github',
    labelString: 'Github',
    icon: 'pi pi-github',
    href: githubUrl,
    visible: true
  },
  {
    label: 'Chat',
    labelString: 'Chat',
    icon: 'pi pi-discord',
    href: discordInvite,
    visible: () => window.innerWidth < 240
  },
  {
    label: 'Soundcloud',
    labelString: 'Soundcloud',
    href: soundcloudUrl,
    iconSvg: () => (isDarkMode.value ? '/soundcloud-logo-white.svg' : '/soundcloud-logo-black.svg'),
    visible: true
  }
]

onMounted(async () => {
  await authStore.loadIfNotInitialized()
})

const toggleMenu = (event: MouseEvent) => {
  navMenuRef.value?.toggle(event)
}
</script>
