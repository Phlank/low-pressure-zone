<template>
  <div class="home-timeslot-grid">
    <DataTable
      :loading="schedules.isLoading"
      :value="rows">
      <template #empty> No DJs have signed up for this schedule yet. </template>
      <Column header="Time">
        <template #body="{ data }: { data: TimeslotRow }">
          <SlotTime
            v-if="data.start"
            :date="data.start"
            style="width: max-content" />
        </template>
      </Column>
      <Column
        field="timeslot.performer"
        header="Performer"
        style="width: 100%">
        <template #body="{ data }: { data: TimeslotRow }">
          <ListItem v-if="data.timeslot">
            <template #left>
              <SlotName
                :name="data.timeslot.subtitle ?? ''"
                :performer="data.timeslot.performer.name" />
            </template>
            <template #right>
              <a
                v-if="data.timeslot?.performer.url"
                :href="data.timeslot.performer.url">
                <i class="pi pi-external-link"></i>
              </a>
            </template>
          </ListItem>
        </template>
      </Column>
      <Column
        v-if="!isMobile"
        header="Type">
        <template #body="{ data }: { data: TimeslotRow }">
          <div style="width: max-content">{{ data.timeslot?.performanceType }}</div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script lang="ts" setup>
import SlotName from '@/components/controls/SlotName.vue'
import SlotTime from '@/components/controls/SlotTime.vue'
import { Column, DataTable } from 'primevue'
import ListItem from '@/components/data/ListItem.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { computed, inject, ref, type Ref, watch } from 'vue'
import type { TimeslotResponse } from '@/api/resources/timeslotsApi.ts'
import {
  getNextHour,
  getPreviousHour,
  isDateInTimeslot,
  parseDate,
  timesBetween
} from '@/utils/dateUtils.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const schedules = useScheduleStore()

const props = defineProps<{
  scheduleId: string
}>()

const schedule = computed(() => schedules.getScheduleById(props.scheduleId))
interface TimeslotRow {
  start: Date
  timeslot?: TimeslotResponse
}
const rows: Ref<TimeslotRow[]> = ref([])
const setupRows = () => {
  const newRows: TimeslotRow[] = []
  if (!schedule.value || schedule.value.timeslots.length === 0) {
    rows.value = newRows
    return
  }
  const timeslots = schedule.value.timeslots
  const startFirst = parseDate(timeslots[0]!.startsAt)
  const endLast = parseDate(timeslots[schedule.value.timeslots.length - 1]!.endsAt)
  const hours = timesBetween(startFirst, endLast, 60)
  if (startFirst > parseDate(schedule.value.startsAt)) hours.unshift(getPreviousHour(startFirst))
  if (endLast < parseDate(schedule.value.endsAt)) hours.push(getNextHour(endLast))

  hours.forEach((hour) => {
    const timeslot = timeslots.find((timeslot) => isDateInTimeslot(hour, timeslot))
    newRows.push({
      start: hour,
      timeslot: timeslot
    })
  })
  rows.value = newRows
}

watch(
  () => schedule.value,
  () => {
    setupRows()
  },
  { immediate: true, deep: true }
)
</script>
