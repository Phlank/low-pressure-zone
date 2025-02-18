<template>
  <Toolbar class="header">
    <template #start>
      <RouterLink
        :to="'/'"
        class="header__title">
        Low Pressure Zone
      </RouterLink>
    </template>
    <template #end>
      <DarkModeToggle class="header__dark-mode-toggle" />
      <SiteNavMenu />
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Toolbar } from 'primevue'
import DarkModeToggle from '../controls/DarkModeToggle.vue'
import SiteNavMenu from '../controls/SiteNavMenu.vue'
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
