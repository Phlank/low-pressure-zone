<template>
  <Toolbar
    class="site-footer"
    style="border-style: none; background-color: inherit">
    <template #center>
      <PlayButton />
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Toolbar } from 'primevue'
import PlayButton from '../../controls/PlayButton.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.site-footer {
  position: sticky;
  bottom: 0;
}
</style>
