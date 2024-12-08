<template>
  <div class="app">
    <SiteHeader class="header" />
    <RouterView class="content" />
    <SiteFooter class="footer" />
  </div>
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import SiteHeader from './components/shared/SiteHeader.vue'
import SiteFooter from './components/shared/SiteFooter.vue'
</script>

<style lang="scss" scoped>
.app {
  display: flex;
  flex-direction: column;
  flex-flow: column;
  height: 100vh;
}

.content {
  margin-top: auto;
  margin-bottom: auto;
}
</style>
