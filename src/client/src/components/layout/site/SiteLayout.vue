<template>
  <div class="site-layout">
    <div>
      <SiteHeader class="site-layout__header" />
      <main class="site-layout__view">
        <RouterView class="site-layout__content" />
      </main>
      <SiteFooter class="site-layout__footer" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { RouterView } from 'vue-router'
import SiteFooter from './SiteFooter.vue'
import SiteHeader from './SiteHeader.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.site-layout {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100dvh;

  &__header {
    margin-bottom: variables.$space-m;
    font-size: 1.2rem;
    position: sticky;
    top: 0;
    z-index: 100;
    border-top: 0;
    border-left: 0;
    border-right: 0;
    border-radius: 0;
  }

  &__view {
    min-height: variables.$view-min-height;
  }

  &__footer {
    position: sticky;
    text-align: center;
    width: 100%;
  }
}
</style>
