<template>
  <TwoLineData
    :bottom="formatReadableTime(utcDate) + ' UTC'"
    :top="formatReadableTime(date)"
    class="slot-time" />
</template>

<script lang="ts" setup>
import { formatReadableTime } from '@/utils/dateUtils.ts'
import { computed } from 'vue'
import TwoLineData from '@/components/layout/TwoLineData.vue'

const props = defineProps<{
  date: Date
}>()
const utcDate = computed(() => {
  return new Date(props.date.getTime() + props.date.getTimezoneOffset() * 60 * 1000)
})
</script>
