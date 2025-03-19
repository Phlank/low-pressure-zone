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
import { computed, onMounted, onUnmounted, ref, useTemplateRef } from 'vue'
import { clamp, useResizeObserver } from '@vueuse/core'

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
// xs: 2, s: 3, m: 4, l: 6, xl: 8
// Thinking about this, 1fr should be ~75px-150px.
// Column gap is 20px, so each column should be considered 55px.
const columns = computed(() => clamp(Math.floor(width.value / 80), 1, 99))

const gridColsStyle = computed(() => `repeat(${columns.value}, 1fr)`)

onUnmounted(() => {})
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.form-area {
  &__fields {
    display: grid;
    grid-template-columns: v-bind(gridColsStyle);
    column-gap: variables.$space-l;
  }
}
</style>
