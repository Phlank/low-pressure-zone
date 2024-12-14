<template>
  <div class="app">
    <SiteHeader class="header" />
    <ContentLayout class="content" />
    <SiteFooter class="footer" />
  </div>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import SiteHeader from '@/components/layout/SiteHeader.vue'
import SiteFooter from '@/components/layout/SiteFooter.vue'
import ContentLayout from './components/layout/ContentLayout.vue'
import { onMounted, computed, provide, type ComputedRef, ref, type Ref } from 'vue'

const screenWidth: Ref<number> = ref(1000)

onMounted(() => {
  window.addEventListener('resize', () => {
    screenWidth.value = window.screen.width
    console.log(`New screenwidth: ${window.screen.width} | isMobile: ${isMobile.value}`)
  })
})

const mobileWidth = 760
const isMobile: ComputedRef<boolean> = computed(() => screenWidth.value <= mobileWidth)
provide('isMobile', isMobile)
</script>

<style lang="scss" scoped>
.app {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.content {
  margin: 0 0 auto 0;
}
</style>
