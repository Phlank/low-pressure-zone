<template>
  <ContentPanel class="header panel">
    <div class="header_left">The Wayward Inn presents</div>
    <div class="header_center">Low Pressure Zone</div>
    <div class="header_right">Link to other sources</div>
  </ContentPanel>
</template>

<script lang="ts" setup>
import ContentPanel from './ContentPanel.vue'
</script>

<style lang="scss" scoped>
@import './../../assets/variables.scss';

.header {
  display: flex;
  align-items: center;
  justify-content: space-between;

  &_left {
    margin-right: auto;
    color: $font-color-darker;
  }

  &_center {
    color: $font-color-brighter;
    font-size: larger;
  }

  &_right {
    margin-left: auto;
  }
}
</style>
