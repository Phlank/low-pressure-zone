<template>
  <div class="app">
    <Toast class="app__toast" />
    <SiteLayout />
  </div>
</template>

<script setup lang="ts">
import { Toast } from 'primevue'
import { computed, onMounted, onUnmounted, provide, ref, type ComputedRef, type Ref } from 'vue'
import SiteLayout from './components/layout/site/SiteLayout.vue'

const screenWidth: Ref<number> = ref(1000)

onMounted(() => {
  updateScreenWidth()
  window.addEventListener('resize', updateScreenWidth)
})

onUnmounted(() => {
  window.removeEventListener('resize', updateScreenWidth)
})

const updateScreenWidth = () => {
  screenWidth.value = window.screen.width
}

const mobileWidth = 760
const isMobile: ComputedRef<boolean> = computed(() => screenWidth.value <= mobileWidth)
provide('isMobile', isMobile)
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';
.app {
  &__toast {
    max-width: calc(100vw - 2 * variables.$space-l);
  }
}
</style>
