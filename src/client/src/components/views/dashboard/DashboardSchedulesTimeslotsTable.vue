<template>
  <DataTable data-key="id" :value="schedule.timeslots">
    <Column field="start" header="Start">
      <template #body="timeslotProps">
        {{ timeslotProps.data.start.toLocaleDateString() }}
      </template>
    </Column>
    <Column field="performer.name" header="Performer" />
  </DataTable>
</template>

<script lang="ts" setup>
import { DataTable, Column } from 'primevue'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import { hoursBetween } from '@/utils/dateUtils'
import { ref } from 'vue'
import type { TimeslotResponse } from '@/api/schedules/timeslots/timeslotResponse'

const props = defineProps<{
  schedule: ScheduleResponse
}>()

const startDate = ref(new Date(Date.parse(props.schedule.start)))
const endDate = ref(new Date(Date.parse(props.schedule.end)))

interface TimeslotRow {
  startTime: Date
  timeslot?: TimeslotResponse
}

const setupRows = () => {
  const rows: TimeslotRow[] = []
  const hours = hoursBetween(startDate.value, endDate.value)
  hours.forEach((hour) => {
    rows.push({
      startTime: hour,
      timeslot: props.schedule.timeslots.find((timeslot) => timeslot.start === hour.toISOString())
    })
  })
}
</script>
