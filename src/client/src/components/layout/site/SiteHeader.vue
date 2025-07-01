<template>
  <Toolbar class="site-header">
    <template #start>
      <RouterLink
        :to="'/'"
        class="site-header__title">
        <img
          ref="logo"
          v-ripple
          :src="logoSrc"
          alt="Low Pressure Zone"
          @mousedown="logoSrc = '/site-logo-click.png'"
          @mouseenter="logoSrc = '/site-logo-hover.png'"
          @mouseleave="logoSrc = '/site-logo.png'"
          @mouseup="logoSrc = '/site-logo-hover.png'" />
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
import { ref, useTemplateRef } from 'vue'
import { onClickOutside } from '@vueuse/core'

const logoSrc = ref('/site-logo.png')
const logo = useTemplateRef('logo')

onClickOutside(logo, () => {
  logoSrc.value = '/site-logo.png'
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.site-header {
  &__title {
    vertical-align: middle;

    img {
      display: block;
      height: 46px;
    }
  }

  &__dark-mode-toggle {
    @media (width < 360px) {
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
