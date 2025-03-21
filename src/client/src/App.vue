<template>
  <div
    ref="appRef"
    class="app">
    <Toast class="app__toast" />
    <SiteLayout />
  </div>
</template>

<script lang="ts" setup>
import { Toast } from 'primevue'
import { provide, ref, type Ref } from 'vue'
import SiteLayout from './components/layout/site/SiteLayout.vue'
import { useResizeObserver } from '@vueuse/core'
import { mobileWidth } from '@/constants/size.ts'

const isMobile: Ref<boolean> = ref(false)
useResizeObserver(document.body, () => {
  isMobile.value = document.body.offsetWidth <= mobileWidth
})
provide('isMobile', isMobile)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.app {
  min-width: 100vw;

  &__toast {
    max-width: calc(100vw - 2 * #{variables.$space-l});
  }
}
</style>
