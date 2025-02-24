<template>
  <Toolbar
    class="footer"
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

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.footer {
  position: sticky;
  bottom: 0;
  padding: variables.$space-l;
}
</style>
