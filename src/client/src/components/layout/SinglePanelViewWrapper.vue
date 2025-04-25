<template>
  <div class="single-panel-view-wrapper">
    <div class="single-panel-view-wrapper__inner">
      <Panel
        :class="`single-panel-center ${header ? '' : 'single-panel-center--no-header'}`"
        :header="header">
        <slot></slot>
      </Panel>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Panel } from 'primevue'

defineProps<{
  header?: string
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.single-panel-view-wrapper {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  align-items: center;
  margin: auto;
  min-height: variables.$view-min-height;

  &__inner {
    margin: auto;
  }
}

.single-panel-center {
  width: min(340px, calc(100vw - 2 * #{variables.$space-l}));

  &--no-header {
    .p-panel-header {
      display: none;
    }

    .p-panel-content {
      padding: 1.125rem;
    }
  }
}
</style>
