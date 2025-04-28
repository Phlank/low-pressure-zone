<template>
  <div class="home-view flex-variable-space-between">
    <UpcomingSchedules class="flex-variable-space-between__left" />
  </div>
</template>

<script lang="ts" setup>
import UpcomingSchedules from './UpcomingSchedules.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.home-view {
  margin: 0 variables.$space-m;
  @include variables.desktop {
    max-width: calc(#{variables.$mobile-element-width} + 200px);
  }
}
</style>
