<template>
  <div class="expandable-content">
    <div class="expandable-content__top">
      <slot name="top"></slot>
    </div>
    <div
      ref="bottomElementRef"
      :class="bottomClass">
      <slot name="bottom"></slot>
    </div>
    <div class="expandable-content__show-more">
      <Button
        :label="`Read ${isShowingMore ? 'Less' : 'More'}`"
        class="expandable-content__show-more__button"
        outlined
        severity="secondary"
        @click="isShowingMore = !isShowingMore" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Button } from 'primevue'
import { computed, ref } from 'vue'
import { useMutationObserver, useResizeObserver } from '@vueuse/core'

const isShowingMore = ref(false)
const bottomElementRef = ref<HTMLElement | null>(null)
const bottomClass = computed(
  () =>
    `expandable-content__bottom expandable-content__bottom--${isShowingMore.value ? 'visible' : 'hidden'}`
)
const bottomHeight = ref('0px')
const recalculateHeight = () => {
  const newHeight =
    (document.getElementsByClassName('expandable-content__bottom')[0]?.scrollHeight ?? 0) + 'px'
  if (newHeight !== bottomHeight.value) {
    bottomHeight.value = newHeight
  }
}
useResizeObserver(document.body, () => {
  recalculateHeight()
})

useMutationObserver(
  bottomElementRef,
  () => {
    recalculateHeight()
  },
  {
    subtree: true,
    childList: true,
    characterData: true
  }
)
</script>

<style lang="scss" scoped>
.expandable-content {
  &__bottom {
    overflow: hidden;
    transition: max-height 0.15s ease-in-out;

    &--hidden {
      max-height: 0;
    }

    &--visible {
      max-height: v-bind(bottomHeight);
    }

    :first-child {
      margin-top: 0;
    }

    :last-child {
      margin-bottom: 0;
    }
  }

  &__show-more {
    display: flex;
    justify-content: center;

    &__button {
      margin-top: 1rem;
    }
  }
}
</style>
