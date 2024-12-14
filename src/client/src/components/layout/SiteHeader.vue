<template>
  <ContentPanel class="site-header">
    <div class="left">The Wayward Inn presents</div>
    <div class="center header">Low Pressure Zone</div>
    <div class="right">Online bass radio</div>
  </ContentPanel>
</template>

<script lang="ts" setup>
import ContentPanel from '@/components/shared/ContentPanel.vue'
</script>

<style lang="scss" scoped>
@import '@/assets/main.scss';
@import '@/assets/variables.scss';

.site-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
}

.left {
  margin-right: auto;
  color: $font-color-white-darker;
  width: 33.3vw;
}

.center {
  color: $font-color-white-brighter;
  font-size: larger;
  width: 33.4vw;
  text-align: center;
}

.right {
  margin-left: auto;
  color: $font-color-white-darker;
  width: 33.3vw;
  text-align: right;
}
</style>
