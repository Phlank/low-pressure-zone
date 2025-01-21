<template>
  <div v-if="isMobile" class="content-layout-mobile">
    <RouterView />
  </div>
  <div v-else class="content-layout">
    <RouterView class="content-layout__view" />
    <iframe
      src="https://discord.com/widget?id=722643085137412096&theme=dark"
      width="350"
      allowtransparency="true"
      frameborder="0"
      sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"
      class="content-layout__chat"
    ></iframe>
  </div>
</template>

<script lang="ts" setup>
import { inject } from 'vue'
import { RouterView } from 'vue-router'

const isMobile = inject<boolean>('isMobile')
</script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

.content-layout {
  display: flex;
  justify-content: space-between;
  margin: 0 variables.$space-s;
  max-height: calc(100vh - 64px);
  overflow-y: scroll;

  &__view {
    min-width: 30%;
    max-width: 40%;
  }

  &__chat {
    border-radius: variables.$space-s;
  }
}

.content-layout-mobile {
  margin: 0 variables.$space-m;
}
</style>
