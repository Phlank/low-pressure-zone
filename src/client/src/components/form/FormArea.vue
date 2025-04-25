<template>
  <div
    ref="formAreaRef"
    class="form-area">
    <div
      v-if="header"
      class="form-area__header">
      <slot name="header">
        <h4>{{ header }}</h4>
      </slot>
    </div>
    <div class="form-area__fields">
      <slot></slot>
    </div>
    <div :class="`form-area__actions ${isSingleColumn ? 'form-area__actions--single-column' : ''}`">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, ref, useTemplateRef } from 'vue'
import { clamp, useResizeObserver } from '@vueuse/core'
import { mobileWidth } from '@/constants/size.ts'

const formAreaRef = useTemplateRef('formAreaRef')
const width = ref(0)

withDefaults(
  defineProps<{
    header?: string
    isSingleColumn?: boolean
  }>(),
  {
    isSingleColumn: false
  }
)

onMounted(() => {
  if (formAreaRef.value) {
    width.value = formAreaRef.value.offsetWidth
  }
})

useResizeObserver(formAreaRef, (entries) => {
  const entry = entries[0]
  width.value = entry.contentRect.width
})

const columnWidth = 80
const columns = computed(() => {
  if (width.value <= mobileWidth) {
    return clamp(Math.floor(width.value / columnWidth), 1, 4)
  }
  return clamp(Math.floor(width.value / columnWidth), 1, 99)
})
const widthPx = computed(() => {
  if (width.value <= mobileWidth) {
    return width.value + 'px'
  }
  return width.value - (width.value % columnWidth) + 'px'
})
const gridColsStyle = computed(() => `repeat(${columns.value}, 1fr)`)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.form-area {
  &__fields {
    width: min(v-bind(widthPx), 100%);
    display: grid;
    grid-template-columns: v-bind(gridColsStyle);
    column-gap: variables.$space-l;
  }

  &__actions {
    display: flex;
    flex-direction: row;
    gap: variables.$space-m;

    &--single-column {
      flex-direction: column;
    }
  }
}
</style>
