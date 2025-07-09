<template>
  <div class="dashboard-view flex-variable-space-between">
    <Menu
      v-if="!isMobile"
      :model="menuItems"
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height"
      z-index="99">
      <template #item="{ item, props }">
        <div>
          <RouterLink
            v-if="item.route"
            v-slot="{ href, navigate }"
            :to="item.route"
            custom>
            <a
              v-if="item.route"
              v-ripple
              :href="href"
              v-bind="props.action"
              @click="navigate">
              <span :class="item.icon" />
              <span>{{ item.label }}</span>
            </a>
            <a
              v-else
              v-ripple
              :href="item.url"
              :target="item.target"
              v-bind="props.action">
              <span :class="item.icon" />
              <span>{{ item.label }}</span>
            </a>
          </RouterLink>
        </div>
      </template>
    </Menu>
    <Panel
      :header="isMobile ? '' : ((router.currentRoute.value.name as string) ?? '')"
      class="dashboard-view__content flex-variable-space-between__right flex-variable-space-between__right--full-width">
      <TitledNavMenu
        v-if="isMobile"
        :items="menuItems" />
      <RouterView />
    </Panel>
  </div>
</template>

<script lang="ts" setup>
import TitledNavMenu from '@/components/controls/TitledNavMenu.vue'
import { useAuthStore } from '@/stores/authStore'
import { Menu, Panel } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { inject, onMounted, type Ref } from 'vue'
import { useRouter } from 'vue-router'
import roles from '@/constants/roles.ts'

const authStore = useAuthStore()
const router = useRouter()

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const menuItems: MenuItem[] = [
  {
    label: 'Schedules',
    icon: 'pi pi-calendar',
    route: '/dashboard',
    visible: true
  },
  {
    label: 'Communities',
    icon: 'pi pi-globe',
    route: '/dashboard/communities',
    visible: true
  },
  {
    label: 'Performers',
    icon: 'pi pi-microphone',
    route: '/dashboard/performers',
    visible: true
  },
  {
    label: 'Broadcasts',
    icon: 'pi pi-receipt',
    route: '/dashboard/broadcasts',
    visible: authStore.isInRole(roles.admin) || authStore.isInRole(roles.organizer)
  },
  {
    label: 'Stream Setup',
    icon: 'pi pi-wave-pulse',
    route: '/dashboard/streaming',
    visible: true
  },
  {
    label: 'Users',
    icon: 'pi pi-user',
    route: '/dashboard/users',
    visible: authStore.isInRole(roles.admin) || authStore.isInRole(roles.organizer)
  }
]

onMounted(async () => {
  await authStore.loadIfNotInitialized()
  if (!authStore.isLoggedIn()) {
    await router.push('/')
  }
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.dashboard-view {
  &__content {
    @include variables.mobile {
      padding-bottom: 0;
    }
  }
}
</style>
