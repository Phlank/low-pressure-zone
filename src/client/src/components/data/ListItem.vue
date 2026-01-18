<template>
  <div
    class="list-item"
    @click="emit('click')">
    <div :class="leftClass">
      <slot name="left"></slot>
    </div>
    <div :class="rightClass">
      <slot name="right"></slot>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { computed } from 'vue'

const emit = defineEmits<{
  click: []
}>()

const props = withDefaults(
  defineProps<{
    hideOverflowLeft?: boolean
    hideOverflowRight?: boolean
  }>(),
  {
    hideOverflowLeft: true,
    hideOverflowRight: false
  }
)

const leftClass = computed(() =>
  props.hideOverflowLeft ? 'list-item__left list-item__left--no-overflow' : 'list-item__left'
)

const rightClass = computed(() =>
  props.hideOverflowRight ? 'list-item__right list-item__right--no-overflow' : 'list-item__right'
)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.list-item {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  flex-wrap: nowrap;
  width: 100%;
  min-height: 4rem;
  gap: variables.$space-l;

  &__left {
    display: flex;
    flex-direction: column;

    &--no-overflow {
      overflow: hidden;

      span {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }
  }

  &__right {
    display: flex;
    flex-direction: column;

    &--no-overflow {
      overflow: hidden;

      span {
        overflow: hidden;
        text-overflow: ellipsis;
        white-space: nowrap;
      }
    }
  }
}
</style>
