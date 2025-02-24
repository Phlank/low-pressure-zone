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

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.list-item {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  user-select: none;
  // padding: variables.$space-m 0;

  &__right {
    margin-left: auto;
  }
}
</style>
