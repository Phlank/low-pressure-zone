<template>
  <Toolbar class="header">
    <template #start>
      <RouterLink :to="'/'" class="header__title"> Low Pressure Zone </RouterLink>
    </template>
    <template #end>
      <DarkModeToggle class="header__dark-mode-toggle" />
      <div v-if="isMobile" style="display: flex">
        <Button
          icon="pi pi-bars"
          size="large"
          @click="toggleMenu"
          outlined
          aria-controls="navigation-menu"
        />
        <Menu id="navigation-menu" ref="navMenuRef" :model="navMenuItems" :popup="true">
          <!-- https://primevue.org/menu/#router -->
          <template #item="{ item, props }">
            <router-link v-if="item.route" v-slot="{ href, navigate }" :to="item.route" custom>
              <a v-ripple :href="href" v-bind="props.action" @click="navigate">
                <span :class="item.icon" />
                <span class="ml-2">{{ item.label }}</span>
              </a>
            </router-link>
            <a v-else v-ripple :href="item.url" :target="item.target" v-bind="props.action">
              <span :class="item.icon" />
              <span class="ml-2">{{ item.label }}</span>
            </a>
          </template>
        </Menu>
      </div>
      <div v-else>
        <RouterLink v-for="item in navMenuItems" :key="item.labelString" :to="item.route">
          <Button :label="item.labelString" outlined />
        </RouterLink>
      </div>
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Button, Toolbar, Menu } from 'primevue'
import { inject, useTemplateRef, type Ref } from 'vue'
import DarkModeToggle from '../controls/DarkModeToggle.vue'
import type { MenuItem } from 'primevue/menuitem'

const isMobile: Ref<boolean> | undefined = inject('isMobile')

const navMenuRef = useTemplateRef('navMenuRef')
const navMenuItems: MenuItem[] = [
  {
    label: 'Dashboard',
    labelString: 'Dashboard',
    icon: 'pi pi-cog',
    route: '/dashboard'
  }
]

const toggleMenu = (event: MouseEvent) => {
  navMenuRef.value?.toggle(event)
}
</script>

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.header {
  &__dark-mode-toggle {
    @include variables.tiny {
      display: none;
    }
  }
}

.p-button {
  margin: 0 variables.$space-s;
}

:deep(.p-toolbar-start) {
  a {
    text-decoration: none;
    color: inherit;
  }
}
</style>
