<template>
  <div class="site-layout">
    <div>
      <SiteHeader class="site-layout__header" />
      <main>
        <RouterView class="site-layout__content" />
      </main>
    </div>
    <SiteFooter class="site-layout__footer" />
  </div>
</template>

<script lang="ts" setup>
import SiteHeader from './SiteHeader.vue'
import SiteFooter from './SiteFooter.vue'
</script>

<style lang="scss" scoped>
@use '@/assets/variables.scss';

.site-layout {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100vh;

  &__header {
    margin-bottom: variables.$space-m;
  }

  &__footer {
    margin-top: variables.$space-m;
  }
}
</style>
