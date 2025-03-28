<template>
  <div class="flex-variable-space-between">
    <UpcomingSchedules
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height" />
    <iframe
      class="discord-iframe flex-variable-space-between__right"
      height="500"
      sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"
      src="https://discord.com/widget?id=722643085137412096&theme=dark"
      title="discord"
      width="350"></iframe>
  </div>
</template>

<script lang="ts" setup>
import UpcomingSchedules from './UpcomingSchedules.vue'
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.discord-iframe {
  border: none;
  background-color: transparent;
  border-radius: variables.$space-m;
}
</style>
