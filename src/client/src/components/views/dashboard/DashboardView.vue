<template>
  <div class="flex-variable-space-between">
    <Menu v-if="!isMobile" class="flex-variable-space-between__left" :model="menuItems" />
    <Panel
      class="flex-variable-space-between__right flex-variable-space-between__right--full-width"
      :header="isMobile ? '' : (($router.currentRoute.value.name as string) ?? '')"
    >
      <TitledNavMenu v-if="isMobile" :items="menuItems" />
      <RouterView />
    </Panel>
  </div>
</template>

<script setup lang="ts">
import TitledNavMenu from '@/components/controls/TitledNavMenu.vue'
import { Menu, Panel } from 'primevue'
import type { MenuItem } from 'primevue/menuitem'
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const menuItems: MenuItem[] = [
  {
    label: 'Schedules',
    icon: 'pi pi-calendar',
    route: '/dashboard/schedules'
  },
  {
    label: 'Audiences',
    icon: 'pi pi-globe',
    route: '/dashboard/audiences'
  },
  {
    label: 'Performers',
    icon: 'pi pi-microphone',
    route: '/dashboard/performers'
  }
]
</script>

<style lang="scss" scoped>
@use '@/assets/variables.scss';

.flex-variable-space-between__right {
  :deep(div.p-panel-header) {
    @include variables.mobile {
      padding-bottom: 0;
    }
  }
}
</style>
