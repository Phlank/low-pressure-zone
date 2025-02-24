<template>
  <div class="list-item">
    <div><slot name="left"></slot></div>
    <div><slot name="right"></slot></div>
  </div>
</template>

<style lang="scss" scoped>
@use '@/assets/styles/variables.scss';

.list-item {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: space-between;
  padding: variables.$space-m 0;
}
</style>
