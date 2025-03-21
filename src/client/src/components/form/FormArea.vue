<template>
  <div
    ref="formAreaRef"
    class="form-area">
    <div class="form-area__header">
      <slot name="header">
        <h4>{{ header }}</h4>
      </slot>
    </div>
    <div class="form-area__fields">
      <slot></slot>
    </div>
    <div class="form-area__actions">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, inject, onMounted, onUnmounted, type Ref, ref, useTemplateRef } from 'vue'
import { clamp, useResizeObserver } from '@vueuse/core'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const formAreaRef = useTemplateRef('formAreaRef')
const width = ref(0)

defineProps<{
  header?: string
}>()

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
  if (isMobile?.value) {
    return clamp(Math.floor(width.value / columnWidth), 1, 4)
  }
  return clamp(Math.floor(width.value / columnWidth), 1, 99)
})
const widthPx = computed(() => {
  if (isMobile?.value) {
    return width.value + 'px'
  }
  return width.value - (width.value % columnWidth) + 'px'
})
const gridColsStyle = computed(() => `repeat(${columns.value}, 1fr)`)

onUnmounted(() => {})
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
}
</style>
