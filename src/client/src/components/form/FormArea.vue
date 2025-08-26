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
    <div :class="`form-area__fields ${useSingleColumn ? 'form-area__fields--single-column' : ''}`">
      <slot></slot>
    </div>
    <div
      :class="`form-area__actions ${useSingleColumn ? 'form-area__actions--single-column' : ''}`">
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

const props = withDefaults(
  defineProps<{
    header?: string
    isSingleColumn?: boolean
    alignActions?: 'left' | 'right'
  }>(),
  {
    isSingleColumn: false,
    alignActions: 'left'
  }
)

const useSingleColumn = computed(() => props.isSingleColumn || width.value <= 402)

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
const actionColumnDirection = computed(() => (props.alignActions === 'left' ? 'ltr' : 'rtl'))
const actionColumnSpan = computed(() => `span ${columns.value <= 4 ? 1 : 2}`)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.form-area {
  &__fields {
    width: min(v-bind(widthPx), 100%);
    display: grid;
    grid-template-columns: v-bind(gridColsStyle);
    column-gap: variables.$space-l;

    &--single-column {
      .form-field {
        grid-column: span 4;
      }
    }
  }

  &__actions {
    width: min(v-bind(widthPx), 100%);
    display: grid;
    grid-template-columns: v-bind(gridColsStyle);
    direction: v-bind(actionColumnDirection);
    column-gap: variables.$space-l;

    &--single-column {
      display: flex;
      gap: variables.$space-m;
      flex-direction: column;
    }

    .p-button {
      grid-column: v-bind(actionColumnSpan);
    }
  }
}
</style>
