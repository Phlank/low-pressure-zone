<template>
  <div class="admin-view flex-variable-space-between">
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
              v-ripple
              :href="href"
              v-bind="props.action"
              @click="navigate">
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
import { inject, type Ref } from 'vue'
import type { MenuItem } from 'primevue/menuitem'
import { Menu, Panel } from 'primevue'
import TitledNavMenu from '@/components/controls/TitledNavMenu.vue'
import { useRouter } from 'vue-router'

const isMobile: Ref<boolean> | undefined = inject<Ref<boolean>>('isMobile')
const router = useRouter()

const menuItems: MenuItem[] = [
  {
    label: 'Content Areas',
    icon: 'pi pi-book',
    route: '/admin',
    visible: true
  }
]
</script>

<style lang="scss" scoped>
@use '@/assets/styles/variables';

.admin-view {
  &__content {
    @include variables.mobile {
      padding-bottom: 0;
    }
  }
}
</style>
