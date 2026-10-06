<template>
  <TwoLineData
    :below="`${isPrerecorded ? 'Prerecorded' : 'Live'}${name ? `: ${name}` : ''}`"
    :above="performer" />
</template>

<script lang="ts" setup>
import TwoLineData from '@/components/layout/TwoLineData.vue'

withDefaults(
  defineProps<{
    name?: string | null
    performer: string
    isPrerecorded: boolean
  }>(),
  {
    name: ''
  }
)
</script>
