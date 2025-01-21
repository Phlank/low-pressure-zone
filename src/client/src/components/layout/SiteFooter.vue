<template>
  <Toolbar class="footer" style="border-style: none; background-color: inherit">
    <template #center>
      <PlayButton />
    </template>
  </Toolbar>
</template>

<script lang="ts" setup>
import { Toolbar } from 'primevue'
import PlayButton from '../controls/PlayButton.vue'
</script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

.footer {
  position: sticky;
  bottom: variables.$space-s;
  padding: variables.$space-m;
}
</style>
