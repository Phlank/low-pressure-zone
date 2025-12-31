<template>
  <TwoLineData
    :bottom="name"
    :top="performer" />
</template>

<script lang="ts" setup>
import TwoLineData from '@/components/layout/TwoLineData.vue'

withDefaults(
  defineProps<{
    name?: string
    performer: string
  }>(),
  {
    name: ''
  }
)
</script>
