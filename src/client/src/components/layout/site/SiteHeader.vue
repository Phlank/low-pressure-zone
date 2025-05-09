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
        <ChatButton class="site-header__chat-button" />
        <SiteNavMenu />
      </div>
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Toolbar } from 'primevue'
import DarkModeToggle from '../../controls/DarkModeToggle.vue'
import SiteNavMenu from '../../controls/SiteNavMenu.vue'
import ChatButton from '@/components/controls/ChatButton.vue'

const title = import.meta.env.VITE_SITE_NAME
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.site-header {
  &__title {
    @media (width < 350px) {
      font-size: 1rem;
    }
  }

  &__dark-mode-toggle {
    @media (width < 472px) {
      display: none;
    }
  }

  &__end {
    align-items: center;
    display: flex;
    gap: variables.$space-m;
  }

  &__chat-button {
    @media (width < 324px) {
      display: none;
    }
  }

  .p-toolbar-start {
    a {
      text-decoration: none;
      color: inherit;
    }
  }
}
</style>
