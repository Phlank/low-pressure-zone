<template>
  <SiteHeader />
  <RouterView />
</template>

<script setup lang="ts">
import { RouterView } from 'vue-router'
import SiteHeader from './components/shared/SiteHeader.vue'
</script>

<style lang="scss" scoped>
@import './assets/variables.scss';

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
