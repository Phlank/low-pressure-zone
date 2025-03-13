<template>
  <div
    class="list-item"
    @click="emit('click')">
    <div class="list-item__left"><slot name="left"></slot></div>
    <div class="list-item__right"><slot name="right"></slot></div>
  </div>
</template>

<script setup lang="ts">
const emit = defineEmits<{
  click: []
}>()
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.list-item {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  user-select: none;
  flex-wrap: nowrap;
  width: 100%;
  gap: variables.$space-l;

  &__left {
    display: flex;
    flex-direction: column;
    overflow: hidden;
    text-overflow: ellipsis;
    white-space: nowrap;
  }

  &__right {
    margin-left: auto;
  }
}
</style>
