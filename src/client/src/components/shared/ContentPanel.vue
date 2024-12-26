<template>
  <div class="panel">
    <slot></slot>
  </div>
</template>

<script lang="ts" setup></script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

$panel-background: variables.$theme-color-darker;
$panel-opacity: 0.8;

.panel {
  padding: variables.$space-m;
  background-color: rgba($panel-background, $panel-opacity);
}
</style>
