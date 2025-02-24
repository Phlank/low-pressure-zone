<template>
  <div class="upcoming-schedules">
    <Skeleton
      v-if="!isLoaded"
      height="250px" />
    <Panel
      v-else
      class="upcoming-schedules"
      :header="title">
      <div
        v-if="schedules.length === 0"
        class="upcoming-schedules__content--none">
        No upcoming schedule to display.
      </div>
      <div
        v-else
        class="upcoming-schedules__content">
        <div class="upcoming-schedules__content__description">{{ scheduleData?.description }}</div>
        <DataTable
          :loading="!isLoaded"
          :value="scheduleData!.timeslots">
          <Column
            field="start"
            header="Time">
            <template #body="{ data }">
              {{ formatTimeslot(data.start) }}
            </template>
          </Column>
          <Column
            field="performer"
            header="Performer" />
          <Column
            v-if="!isMobile"
            field="type"
            header="Type" />
        </DataTable>
      </div>
    </Panel>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import { formatTimeslot, getPreviousHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import { Column, DataTable, Panel, Skeleton, useToast } from 'primevue'
import { computed, inject, onMounted, ref, type ComputedRef, type Ref } from 'vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const schedules: Ref<ScheduleResponse[]> = ref([])
const scheduleIndex: Ref<number> = ref(0)
const isLoaded = ref(false)
const toast = useToast()

interface ScheduleData {
  id: string
  start: Date
  description: string
  audience: string
  timeslots: TimeslotData[]
}

interface TimeslotData {
  start: Date
  performer: string
  type: string
}

const scheduleData: ComputedRef<ScheduleData | undefined> = computed(() => {
  if (schedules.value.length === 0) return undefined
  const schedule = schedules.value[scheduleIndex.value]
  return {
    id: schedule.id,
    start: parseDate(schedule.start),
    description: schedule.description,
    audience: schedule.audience.name,
    timeslots: mapTimeslotDisplayData(schedule)
  }
})

const title: ComputedRef<string> = computed(() => {
  if (scheduleData.value) {
    return `${scheduleData.value.audience} - ${scheduleData.value.start.toLocaleDateString()}`
  }
  return 'Upcoming Schedules'
})

const mapTimeslotDisplayData = (schedule: ScheduleResponse) => {
  const timeslots = schedule.timeslots
  const timeslotData: TimeslotData[] = []
  if (timeslots.length === 0) return timeslotData

  const startFirst = parseDate(timeslots[0].start)
  const endLast = parseDate(timeslots[timeslots.length - 1].end)
  const hours = hoursBetween(startFirst, endLast)
  if (startFirst > parseDate(schedule.start)) {
    hours.unshift(getPreviousHour(startFirst))
  }
  if (endLast < parseDate(schedule.end)) {
    hours.push(endLast)
  }

  hours.forEach((hour) => {
    const slot = timeslots.find((t) => Date.parse(t.start) === hour.getTime())
    timeslotData.push({
      start: hour,
      performer: slot?.performer.name ?? '',
      type: slot?.performanceType ?? ''
    })
  })
  return timeslotData
}

onMounted(async () => {
  const response = await api.schedules.get({ after: new Date().toISOString() })
  if (tryHandleUnsuccessfulResponse(response, toast)) return
  if (response.isSuccess() && response.data) {
    schedules.value = response.data.sort((a, b) => Date.parse(a.end) - Date.parse(b.end))
    isLoaded.value = true
  }
})
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.upcoming-schedules {
  &__content {
    &__description {
      padding: variables.$space-l 0;
    }
  }
}
</style>
