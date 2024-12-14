<template>
  <div class="panel">
    <slot></slot>
  </div>
</template>

<script lang="ts" setup></script>

<style lang="scss" scoped>
@import '@/assets/variables.scss';

$panel-background: $theme-color-darker;
$panel-opacity: 0.8;

.panel {
  padding: $space-m;
  background-color: rgba($panel-background, $panel-opacity);
}
</style>
