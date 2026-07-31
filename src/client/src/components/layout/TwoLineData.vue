<template>
  <div class="two-line-data">
    <div :class="{ 'two-line-data__top': true, 'two-line-data__top__wrapped': wrapTop }">{{ above }}</div>
    <div
      v-if="below"
      class="two-line-data__bottom">
      {{ below }}
    </div>
  </div>
</template>

<script lang="ts" setup>
withDefaults(defineProps<{
  above: string
  below: string
  wrapTop?: boolean
}>(), {
  wrapTop: false
})
</script>

<style lang="scss">
.two-line-data {
  display: flex;
  flex-direction: column;

  &__top {
    color: var(--p-text-color);

    &__wrapped {
      white-space: wrap;
      overflow-wrap: break-word;
    }
  }

  &__bottom {
    color: var(--p-text-muted-color);
    font-size: small;
  }
}
</style>
