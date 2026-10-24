<template>
  <div
    v-if="!isMobile"
    class="grid-row-fill" />
</template>

<script lang="ts" setup>
import { inject, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
</script>

<style lang="scss">
.grid-row-fill {
  grid-column: auto / -1;
}
</style>
