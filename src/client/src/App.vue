<template>
  <div class="app">
    <Toast />
    <SiteLayout />
  </div>
</template>

<script setup lang="ts">
import { Toast } from 'primevue'
import { computed, onMounted, onUnmounted, provide, ref, type ComputedRef, type Ref } from 'vue'
import SiteLayout from './components/layout/SiteLayout.vue'

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
