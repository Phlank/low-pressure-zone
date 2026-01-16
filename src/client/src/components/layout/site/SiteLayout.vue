<template>
  <div class="site-layout">
    <SiteHeader
      class="site-layout__header"
      id="siteHeader"
      ref="header" />
    <div class="content-and-footer">
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
import { computed, inject, ref, type Ref, useTemplateRef } from 'vue'
import { useResizeObserver } from '@vueuse/core'

const header = useTemplateRef('header')
const footer = useTemplateRef('footer')

const headerHeight = ref(0)
useResizeObserver(header, (entries) => {
  const entry = entries[0]!
  headerHeight.value = entry.contentRect.height + 2 * 11.5 + 2 + 10 // padding, border, margin
})
const footerHeight = ref(0)
useResizeObserver(footer, (entries) => {
  const entry = entries[0]!
  footerHeight.value = entry.contentRect.height + 12 * 2 // padding
})
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const footerPosition = computed(() => (!isMobile?.value ? 'sticky' : 'fixed'))
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.site-layout {
  --header-height: v-bind(headerHeight + 'px');
  --footer-height: v-bind(footerHeight + 'px');
  display: flex;
  flex-direction: column;
  min-height: 100dvh;
  align-items: center;
  width: 100%;

  .content-and-footer {
    width: 100%;
    display: flex;
    flex-direction: column;
    align-items: center;
    justify-content: space-between;
    min-height: calc(100dvh - var(--header-height));
  }

  .p-toolbar.site-layout__header {
    width: 100%;
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
    width: 100%;
  }

  &__footer-space {
    height: var(--footer-height);
  }

  &__footer {
    position: v-bind(footerPosition);
    text-align: center;
    bottom: 0;
    width: fit-content;
  }
}
</style>
