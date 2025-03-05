<template>
  <Toolbar class="header">
    <template #start>
      <RouterLink
        :to="'/'"
        class="header__title">
        {{ title }}
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
import DarkModeToggle from '../../controls/DarkModeToggle.vue'
import SiteNavMenu from '../../controls/SiteNavMenu.vue'

const title = import.meta.env.VITE_SITE_NAME
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.site-header {
  &__dark-mode-toggle {
    @include variables.tiny {
      display: none;
    }
  }
  .p-button {
    margin: 0 variables.$space-s;
  }
  .p-toolbar-start {
    a {
      text-decoration: none;
      color: inherit;
    }
  }
}
</style>
