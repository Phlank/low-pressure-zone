<template>
  <div
    ref="formAreaRef"
    class="form-area">
    <div class="form-area__fields">
      <slot></slot>
    </div>
    <div class="form-area__actions">
      <slot name="actions"></slot>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed, onMounted, onUnmounted, ref, useTemplateRef } from 'vue'
import { useResizeObserver } from '@vueuse/core'

const formAreaRef = useTemplateRef('formAreaRef')
const width = ref(0)

onMounted(() => {
  if (formAreaRef.value) {
    width.value = formAreaRef.value.offsetWidth
  }
})

useResizeObserver(formAreaRef, (entries) => {
  const entry = entries[0]
  width.value = entry.contentRect.width
})

const columns = computed(() => {
  if (width.value < 300) return 2
  if (width.value < 450) return 4
  if (width.value < 600) return 6
  if (width.value < 750) return 8
  if (width.value < 900) return 10
  if (width.value < 1050) return 12
  if (width.value < 1200) return 14
  if (width.value < 1350) return 16
  if (width.value < 1500) return 18
  return 20
})

const gridColsStyle = computed(() => `repeat(${columns.value}, 1fr)`)

onUnmounted(() => {})
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.form-area {
  &__fields {
    display: grid;
    grid-template-columns: v-bind(gridColsStyle);
    row-gap: variables.$space-l;
    column-gap: variables.$space-m;
  }
}
</style>
