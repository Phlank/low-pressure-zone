<template>
  <div class="home-soundclash-grid">
    <DataTable
      :value="rows"
      :loading="schedules.isLoading">
      <Column field="start">
        <template #body="{ data }: { data: SoundclashRow }">
          <SlotTime
            v-if="data.start"
            :date="data.start" />
        </template>
      </Column>
      <Column field="soundclash"> </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import type { SoundclashResponse } from '@/api/resources/soundclashApi'
import { computed, ref, type Ref, watch } from 'vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { parseDate, timesBetween } from '@/utils/dateUtils.ts'
import { addHours } from 'date-fns'
import { Column, DataTable } from 'primevue'
import SlotTime from '@/components/controls/SlotTime.vue'

const schedules = useScheduleStore()

const props = defineProps<{
  scheduleId: string
}>()

const schedule = computed(() => schedules.getScheduleById(props.scheduleId))
interface SoundclashRow {
  start: Date
  soundclash?: SoundclashResponse
}
const rows: Ref<SoundclashRow[]> = ref([])
const setupRows = () => {
  const newRows: SoundclashRow[] = []
  if (!schedule.value || schedule.value.soundclashes.length === 0) {
    rows.value = newRows
    return
  }
  const soundclashes = schedule.value.soundclashes
  const startFirst = new Date(soundclashes[0]!.startsAt)
  const endLast = new Date(soundclashes[schedule.value.soundclashes.length - 1]!.endsAt)
  const hours = timesBetween(startFirst, endLast, 120)
  if (startFirst > parseDate(schedule.value.startsAt)) hours.unshift(addHours(startFirst, -2))
  if (endLast < parseDate(schedule.value.endsAt)) hours.push(addHours(endLast, 2))

  hours.forEach((hour) => {
    const soundclash = soundclashes.find(
      (sc) => parseDate(sc.startsAt).getTime() === hour.getTime()
    )
    newRows.push({
      start: hour,
      soundclash: soundclash
    })
  })
}

watch(
  () => schedule.value,
  () => {
    console.log(schedule.value?.soundclashes)
    setupRows()
  },
  { immediate: true, deep: true }
)
</script>
