<template>
  <div class="content-layout">
    <NavigationMenu />
    <div></div>
    <ContentPanel class="router-panel">
      <RouterView />
    </ContentPanel>
  </div>
</template>

<script lang="ts" setup>
import ContentPanel from '../shared/ContentPanel.vue'
import NavigationMenu from './navigation/NavigationMenu.vue'
import { RouterView } from 'vue-router'
</script>

<style lang="scss" scoped>
@use './../../assets/variables.scss';

.content-layout {
  display: flex;
  flex-direction: column;
}

$panel-max-width: calc(100vw - 2 * variables.$space-l);
$panel-min-width: 360px;
$panel-width-mobile: 100%;

.router-panel {
  max-width: $panel-max-width;
  min-width: $panel-min-width;
  margin: 0 auto 0 auto;
}

@media screen and (max-width: variables.$mobile-width) {
  .router-panel {
    min-width: $panel-width-mobile;
    width: $panel-width-mobile;
  }
}
</style>
