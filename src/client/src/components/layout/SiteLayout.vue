<template>
  <div class="site-layout">
    <div>
      <SiteHeader class="header" />
      <ContentLayout class="content" />
    </div>
    <SiteFooter class="footer" />
  </div>
</template>

<script lang="ts" setup>
import ContentLayout from './ContentLayout.vue'
import SiteHeader from './SiteHeader.vue'
import SiteFooter from './SiteFooter.vue'
</script>

<style lang="scss" scoped>
.site-layout {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100vh;
}
</style>
