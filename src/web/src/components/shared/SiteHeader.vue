<template>
  <ContentPanel class="header">
    <div class="left">The Wayward Inn presents</div>
    <div class="center">Low Pressure Zone</div>
    <div class="right">Online bass radio</div>
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
}

.left {
  margin-right: auto;
  color: $font-color-darker;
  width: 33.3vw;
}

.center {
  color: $font-color-brighter;
  font-size: larger;
  width: 33.4vw;
  text-align: center;
}

.right {
  margin-left: auto;
  color: $font-color-darker;
  width: 33.3vw;
  text-align: right;
}
</style>
