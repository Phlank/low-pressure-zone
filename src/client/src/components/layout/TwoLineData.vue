<template>
  <div class="two-line-data">
    <div class="two-line-data__top">{{ top }}</div>
    <div
      v-if="bottom"
      class="two-line-data__bottom">
      {{ bottom }}
    </div>
  </div>
</template>

<script lang="ts" setup>
defineProps<{
  top: string
  bottom: string
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
