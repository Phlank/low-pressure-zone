<template>
  <div class="home-soundclash-grid">
    <DataTable
      :value="rows"
      :loading="schedules.isLoading">
      <Column
        field="start"
        header="Time">
        <template #body="{ data }: { data: SoundclashRow }">
          <SlotTime :date="data.start" style="width: max-content" />
        </template>
      </Column>
      <Column field="soundclash" style="width: 100%">
        <template #body="{ data }: { data: SoundclashRow }">
          <div v-if="data.soundclash" style="text-align: center;">
            <strong style="font-size: large">{{ data.soundclash.performerOne.name }} vs. {{ data.soundclash.performerTwo.name }}</strong>
            <br />
            Rounds: {{ data.soundclash.roundOne }} | {{ data.soundclash.roundTwo }} | {{ data.soundclash.roundThree }}
          </div>
        </template>
      </Column>
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
    console.log('No rows')
    rows.value = newRows
    return
  }
  const soundclashes = schedule.value.soundclashes
  const startFirst = new Date(soundclashes[0]!.startsAt)
  const endLast = new Date(soundclashes[schedule.value.soundclashes.length - 1]!.endsAt)
  const hours = timesBetween(startFirst, endLast, 120)
  if (startFirst > parseDate(schedule.value.startsAt)) hours.unshift(addHours(startFirst, -2))
  if (endLast < parseDate(schedule.value.endsAt)) hours.push(endLast)

  console.log('Hours:', hours)
  hours.forEach((hour) => {
    const soundclash = soundclashes.find(
      (sc) => parseDate(sc.startsAt).getTime() === hour.getTime()
    )
    newRows.push({
      start: hour,
      soundclash: soundclash
    })
  })
  rows.value = newRows
}

watch(
  () => schedule.value,
  (newValue) => {
    console.log(newValue?.soundclashes)
    setupRows()
  },
  { immediate: true, deep: true }
)
</script>
