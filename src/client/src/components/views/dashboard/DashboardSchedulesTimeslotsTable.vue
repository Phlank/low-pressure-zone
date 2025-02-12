<template>
  <DataTable data-key="id" :value="rows">
    <Column field="start" header="Start">
      <template #body="timeslotProps">
        {{ timeslotProps.data.start.toLocaleTimeString() }}
      </template>
    </Column>
    <Column field="timeslot.performer.name" header="Performer" />
  </DataTable>
</template>

<script lang="ts" setup>
import { DataTable, Column } from 'primevue'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import { hoursBetween } from '@/utils/dateUtils'
import { onMounted, ref, type Ref } from 'vue'
import type { TimeslotResponse } from '@/api/schedules/timeslots/timeslotResponse'

const props = defineProps<{
  schedule: ScheduleResponse
}>()

const startDate = ref(new Date(Date.parse(props.schedule.start)))
const endDate = ref(new Date(Date.parse(props.schedule.end)))

interface TimeslotRow {
  start: string
  timeslot?: TimeslotResponse
}

onMounted(() => {
  setupRows()
})

const rows: Ref<TimeslotRow[]> = ref([])
const setupRows = () => {
  const newRows: TimeslotRow[] = []
  const hours = hoursBetween(startDate.value, endDate.value)
  hours.forEach((hour) => {
    newRows.push({
      start: hour.toLocaleTimeString(),
      timeslot: props.schedule.timeslots.find((timeslot) => timeslot.start === hour.toISOString())
    })
  })
  rows.value = newRows
}
</script>
