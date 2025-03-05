<template>
  <div class="flex-variable-space-between">
    <UpcomingSchedules
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height" />
    <iframe
      src="https://discord.com/widget?id=722643085137412096&theme=dark"
      width="350"
      height="500"
      allowtransparency="true"
      frameborder="0"
      sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"
      class="discord-iframe flex-variable-space-between__right"></iframe>
  </div>
</template>

<script setup lang="ts">
import UpcomingSchedules from './UpcomingSchedules.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.discord-iframe {
  border-radius: variables.$space-m;
}
</style>
