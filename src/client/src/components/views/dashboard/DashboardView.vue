<template>
  <div class="flex-variable-space-between">
    <Menu
      v-if="!isMobile"
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height"
      :model="menuItems"
      z-index="99">
      <template #item="{ item, props }">
        <div v-if="userStore.isInAnySpecifiedRole(...item.roles)">
          <RouterLink
            v-if="item.route"
            v-slot="{ href, navigate }"
            :to="item.route"
            custom>
            <a
              v-ripple
              v-if="item.route"
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
      class="flex-variable-space-between__right flex-variable-space-between__right--full-width"
      :header="isMobile ? '' : (($router.currentRoute.value.name as string) ?? '')">
      <TitledNavMenu
        v-if="isMobile"
        :items="menuItems" />
      <RouterView />
    </Panel>
  </div>
</template>

<script setup lang="ts">
import TitledNavMenu from '@/components/controls/TitledNavMenu.vue'
import { allRoles, Role } from '@/constants/roles'
import { useUserStore } from '@/stores/userStore'
import { Menu, Panel } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { inject, onMounted, type Ref } from 'vue'

const userStore = useUserStore()

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const menuItems: MenuItem[] = [
  {
    label: 'Schedules',
    icon: 'pi pi-calendar',
    route: '/dashboard',
    roles: allRoles
  },
  {
    label: 'Audiences',
    icon: 'pi pi-globe',
    route: '/dashboard/audiences',
    roles: allRoles
  },
  {
    label: 'Performers',
    icon: 'pi pi-microphone',
    route: '/dashboard/performers',
    roles: allRoles
  },
  {
    label: 'Users',
    icon: 'pi pi-user',
    route: '/dashboard/users',
    roles: [Role.Admin]
  },
  {
    label: 'Invites',
    icon: 'pi pi-envelope',
    route: '/dashboard/invites',
    roles: [Role.Admin, Role.Organizer]
  }
]

onMounted(async () => {
  await userStore.loadIfNotInitialized()
})
</script>

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.flex-variable-space-between__right {
  :deep(div.p-panel-header) {
    @include variables.mobile {
      padding-bottom: 0;
    }
  }
}
</style>
