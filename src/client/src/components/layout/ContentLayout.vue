<template>
  <main :class="'content-layout' + `${isMobile ? ' content-layout--mobile' : ''}`">
    <RouterView />
  </main>
</template>

<script lang="ts" setup>
import { inject } from 'vue'
import { RouterView } from 'vue-router'

const isMobile = inject<boolean>('isMobile')
</script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

.content-layout {
  position: relative;
  margin: variables.$space-m;

  @include variables.mobile {
    flex-direction: column;
  }
}
</style>
