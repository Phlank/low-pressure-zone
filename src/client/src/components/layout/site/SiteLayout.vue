<template>
  <div class="site-layout">
    <div>
      <SiteHeader class="site-layout__header" />
      <main class="site-layout__view">
        <RouterView class="site-layout__content" />
      </main>
      <div
        v-if="isMobile"
        class="site-layout__footer-space" />
      <SiteFooter
        ref="footer"
        class="site-layout__footer" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { RouterView } from 'vue-router'
import SiteFooter from './SiteFooter.vue'
import SiteHeader from './SiteHeader.vue'
import { computed, inject, ref, type Ref } from 'vue'

const footer: Ref<HTMLDivElement | undefined> = ref(undefined)

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const footerPosition = computed(() => (!isMobile?.value ? 'sticky' : 'absolute'))
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.site-layout {
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  height: 100dvh;

  .p-toolbar.site-layout__header {
    margin-bottom: variables.$space-m;
    font-size: 1.2rem;
    position: sticky;
    top: 0;
    z-index: 100;
    border-top: 0;
    border-left: 0;
    border-right: 0;
    border-radius: 0;
  }

  &__view {
    min-height: calc(100dvh - #{variables.$header-height} - #{variables.$footer-height});
  }

  &__footer-space {
    height: variables.$footer-height;
  }

  &__footer {
    position: v-bind(footerPosition);
    text-align: center;
    bottom: 0;
    width: 100%;
  }
}
</style>
