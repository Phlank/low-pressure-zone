<template>
  <Toolbar class="site-header">
    <template #start>
      <RouterLink
        :to="'/'"
        class="site-header__title">
        {{ title }}
      </RouterLink>
    </template>
    <template #end>
      <div class="site-header__end">
        <DarkModeToggle class="site-header__dark-mode-toggle" />
        <SiteNavMenu />
      </div>
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
  &__end {
    display: flex;
    gap: variables.$space-m;
  }
  .p-toolbar-start {
    a {
      text-decoration: none;
      color: inherit;
    }
  }
}
</style>
