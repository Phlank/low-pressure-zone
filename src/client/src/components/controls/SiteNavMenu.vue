<template>
  <div class="site-nav-menu" style="display: flex">
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
                style="width: 16px; height: 16px"
                :class="item.icon" />
              <img
                v-if="item.iconSvg"
                :src="item.iconSvg()"
                style="width: 16px; height: 16px" />
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
import { useAuthStore } from '@/stores/authStore'
import { Button, Menu } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { inject, onMounted, ref, type Ref, useTemplateRef } from 'vue'
import { RouterLink } from 'vue-router'
import roles from '@/constants/roles.ts'

const navMenuRef = useTemplateRef('navMenuRef')
const auth = useAuthStore()
const discordInvite = import.meta.env.VITE_DISCORD_INVITE_LINK
const githubUrl = import.meta.env.VITE_GITHUB_URL
const soundcloudUrl = import.meta.env.VITE_SOUNDCLOUD_URL
const bandcampUrl = import.meta.env.VITE_BANDCAMP_URL
const donateUrl = import.meta.env.VITE_DONATE_URL
const isDarkMode: Ref<boolean> = inject('isDarkMode', ref(true))

const navMenuItems: MenuItem[] = [
  {
    label: 'Login',
    labelString: 'Login',
    icon: 'pi pi-sign-in',
    route: '/user/login',
    visible: () => !auth.isLoggedIn
  },
  {
    label: 'Dashboard',
    labelString: 'Dashboard',
    icon: 'pi pi-cog',
    route: '/dashboard',
    visible: () => auth.isLoggedIn
  },
  {
    label: 'Admin',
    labelString: 'Admin',
    icon: 'pi pi-sliders-h',
    route: '/admin',
    visible: () => auth.isInRole(roles.admin)
  },
  {
    label: 'Logout',
    labelString: 'Logout',
    icon: 'pi pi-sign-out',
    callback: async () => await auth.logoutAsync(),
    visible: () => auth.isLoggedIn
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
  },
  {
    label: 'Bandcamp',
    labelString: 'Bandcamp',
    href: bandcampUrl,
    iconSvg: () => (isDarkMode.value ? '/bandcamp-logo-white.svg' : '/bandcamp-logo-black.svg'),
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
    label: 'Buy Me a Coffee',
    labelString: 'Buy Me a Coffee',
    href: donateUrl,
    iconSvg: () => (isDarkMode.value ? '/bmc-logo-white.svg' : '/bmc-logo-black.svg'),
    visible: true
  }
]
onMounted(async () => {
  await auth.initializeAsync()
})

const toggleMenu = (event: MouseEvent) => {
  navMenuRef.value?.toggle(event)
}
</script>
