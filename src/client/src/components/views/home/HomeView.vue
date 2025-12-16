<template>
  <div class="home-view">
    <Panel class="home-view__panel">
      <InfoArea />
      <Divider />
      <UpcomingSchedules />
    </Panel>
  </div>
</template>

<script lang="ts" setup>
import UpcomingSchedules from './UpcomingSchedules.vue'
import { Divider, Panel } from 'primevue'
import InfoArea from '@/components/views/home/InfoArea.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.home-view {
  margin: 0 variables.$space-m;
  display: flex;
  flex-direction: row;
  justify-content: center;

  &__panel {
    width: min(100%, calc(#{variables.$mobile-element-width} + 200px));

    .p-panel-header {
      display: none;
    }
  }
}
</style>
