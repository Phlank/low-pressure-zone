<template>
  <div class="upcoming-schedules">
    <Skeleton
      v-if="schedules.isLoading"
      height="250px" />
    <div v-else>
      <ScheduleNavigator
        v-if="schedules.upcomingSchedules.length > 0"
        :schedules="schedules"
        @changeSchedule="handleChangeSchedule" />
      <div
        v-if="schedules.upcomingSchedules.length === 0"
        class="upcoming-schedules__content upcoming-schedules__content--none">
        No upcoming schedule to display.
      </div>
      <div
        v-else
        class="upcoming-schedules__content">
        <div
          v-show="scheduleData?.description"
          class="upcoming-schedules__content__description">
          {{ scheduleData?.description }}
        </div>
        <DataTable
          :loading="schedules.isLoading"
          :value="scheduleData!.timeslots">
          <Column
            field="start"
            header="Time">
            <template #body="{ data }">
              <SlotTime :date="data.start" />
            </template>
          </Column>
          <Column
            field="performer"
            header="Performer">
            <template #body="{ data }: { data: TimeslotData }">
              <ListItem>
                <template #left>
                  <SlotName
                    :name="data.name"
                    :performer="data.performer" />
                </template>
                <template #right>
                  <a
                    v-if="data.performerUrl !== ''"
                    :href="data.performerUrl">
                    <i class="pi pi-external-link"></i>
                  </a>
                </template>
              </ListItem>
            </template>
          </Column>
          <Column
            v-if="!isMobile"
            field="type"
            header="Type" />
        </DataTable>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { getPreviousHour, hoursBetween, isDateInTimeslot, parseDate } from '@/utils/dateUtils'
import { Column, DataTable, Skeleton } from 'primevue'
import { computed, type ComputedRef, inject, ref, type Ref } from 'vue'
import ListItem from '@/components/data/ListItem.vue'
import ScheduleNavigator from '@/components/controls/ScheduleNavigator.vue'
import SlotTime from '@/components/controls/SlotTime.vue'
import SlotName from '@/components/controls/SlotName.vue'
import { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const scheduleIndex: Ref<number> = ref(0)
const schedules = useScheduleStore()

interface ScheduleData {
  id: string
  start: Date
  description: string
  community: string
  timeslots: TimeslotData[]
}

interface TimeslotData {
  start: Date
  performer: string
  performerUrl: string
  name: string
  type: string
}

const handleChangeSchedule = (newId: string) => {
  const index = schedules.upcomingSchedules.findIndex((s) => s.id === newId)
  if (index === -1) return
  scheduleIndex.value = index
}

const scheduleData: ComputedRef<ScheduleData | undefined> = computed(() => {
  if (schedules.upcomingSchedules.length === 0) return undefined
  const schedule = schedules.upcomingSchedules[scheduleIndex.value]!
  return {
    id: schedule.id,
    start: parseDate(schedule.startsAt),
    description: schedule.description,
    community: schedule.community.name,
    timeslots: mapTimeslotDisplayData(schedule)
  }
})

const mapTimeslotDisplayData = (schedule: ScheduleResponse) => {
  const timeslots = schedule.timeslots
  const timeslotData: TimeslotData[] = []
  if (timeslots.length === 0) return timeslotData

  const startFirst = parseDate(timeslots[0]!.startsAt)
  const endLast = parseDate(timeslots[timeslots.length - 1]!.endsAt)
  const hours = hoursBetween(startFirst, endLast)
  if (startFirst > parseDate(schedule.startsAt)) {
    hours.unshift(getPreviousHour(startFirst))
  }
  if (endLast < parseDate(schedule.endsAt)) {
    hours.push(endLast)
  }

  hours.forEach((hour) => {
    const slot = timeslots.find((timeslot) => isDateInTimeslot(hour, timeslot))
    timeslotData.push({
      start: hour,
      performer: slot?.performer.name ?? '',
      performerUrl: slot?.performer.url ?? '',
      name: slot?.name ?? '',
      type: slot?.performanceType ?? ''
    })
  })
  return timeslotData
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.upcoming-schedules {
  width: 100%;

  &__content {
    margin-top: variables.$space-m;
    text-align: center;

    &__description {
      padding: variables.$space-l 0;
    }
  }
}
</style>
