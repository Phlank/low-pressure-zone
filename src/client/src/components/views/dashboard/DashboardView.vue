<template>
  <div class="dashboard-view flex-variable-space-between">
    <Menu
      v-if="!isMobile"
      :model="menuItems"
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height"
      z-index="99">
      <template #item="{ item, props }">
        <div v-if="authStore.isInAnySpecifiedRole(...item.roles)">
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
import { Role } from '@/constants/role.ts'
import { useAuthStore } from '@/stores/authStore'
import { Menu, Panel } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { inject, onMounted, type Ref } from 'vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useRouter } from 'vue-router'

const authStore = useAuthStore()
const communityStore = useCommunityStore()
const router = useRouter()

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const menuItems: MenuItem[] = [
  {
    label: 'Schedules',
    icon: 'pi pi-calendar',
    route: '/dashboard',
    roles: []
  },
  {
    label: 'Communities',
    icon: 'pi pi-globe',
    route: '/dashboard/communities',
    roles: []
  },
  {
    label: 'Performers',
    icon: 'pi pi-microphone',
    route: '/dashboard/performers',
    roles: []
  },
  {
    label: 'Users',
    icon: 'pi pi-user',
    route: '/dashboard/users',
    roles: [Role.Admin]
  }
]

onMounted(async () => {
  const promises: Promise<void>[] = []
  promises.push(authStore.loadIfNotInitialized())
  promises.push(communityStore.loadCommunitiesAsync())
  await Promise.all(promises)
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
