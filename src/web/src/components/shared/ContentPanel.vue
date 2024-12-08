<template>
  <div class="panel">
    <slot></slot>
  </div>
</template>

<script lang="ts" setup></script>

<style lang="scss" scoped>
@import './../../assets/variables.scss';
.panel {
  padding: $space-m;
  background-color: rgba($theme-color-darker, 0.6);
}
</style>
