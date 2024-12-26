<template>
  <div class="site-layout">
    <SiteHeader class="header" />
    <ContentLayout class="content" />
    <SiteFooter class="footer" />
  </div>
</template>

<script lang="ts" setup>
import SiteHeader from './SiteHeader.vue'
import SiteFooter from './SiteFooter.vue'
import ContentLayout from './ContentLayout.vue'
</script>

<style lang="scss" scoped>
.site-layout {
  display: flex;
  flex-direction: column;
  height: 100vh;
}

.content {
  margin: 0 0 auto 0;
}
</style>
