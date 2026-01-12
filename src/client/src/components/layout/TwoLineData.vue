<template>
  <div class="two-line-data">
    <div class="two-line-data__top">{{ above }}</div>
    <div
      v-if="below"
      class="two-line-data__bottom">
      {{ below }}
    </div>
  </div>
</template>

<script lang="ts" setup>
defineProps<{
  above: string
  below: string
}>()
</script>

<style lang="scss">
.two-line-data {
  display: flex;
  flex-direction: column;

  &__top {
    color: var(--p-text-color);
  }

  &__bottom {
    color: var(--p-text-muted-color);
    font-size: small;
  }
}
</style>
