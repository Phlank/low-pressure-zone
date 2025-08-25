<template>
  <div class="slot-time">
    <div class="slot-time__local">{{ formatReadableTime(date) }}</div>
    <div class="slot-time__gmt">{{ formatReadableTime(gmtDate) }} UTC</div>
  </div>
</template>

<script lang="ts" setup>
import { formatReadableTime } from '@/utils/dateUtils.ts'
import { computed } from 'vue'

const props = defineProps<{
  date: Date
}>()
const gmtDate = computed(() => {
  return new Date(props.date.getTime() + props.date.getTimezoneOffset() * 60 * 1000)
})
</script>

<style lang="scss">
.slot-time {
  display: flex;
  flex-direction: column;

  &__local {
    color: var(--p-text-color);
  }

  &__gmt {
    color: var(--p-text-muted-color);
    font-size: small;
  }
}
</style>
