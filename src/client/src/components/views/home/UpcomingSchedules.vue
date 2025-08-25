<template>
  <div class="upcoming-schedules">
    <Skeleton
      v-if="!isLoaded"
      height="250px" />
    <div v-else>
      <ScheduleNavigator
        v-if="schedules.length > 0"
        :schedules="schedules"
        @changeSchedule="handleChangeSchedule" />
      <div
        v-if="schedules.length === 0"
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
          :loading="!isLoaded"
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
                <template #left>{{ data.performer }}</template>
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
import { getPreviousHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import { Column, DataTable, Skeleton, useToast } from 'primevue'
import { computed, type ComputedRef, inject, onMounted, ref, type Ref } from 'vue'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import ListItem from '@/components/data/ListItem.vue'
import ScheduleNavigator from '@/components/controls/ScheduleNavigator.vue'
import SlotTime from '@/components/controls/SlotTime.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const schedules: Ref<ScheduleResponse[]> = ref([])
const scheduleIndex: Ref<number> = ref(0)
const isLoaded = ref(false)
const toast = useToast()

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
  type: string
}

const handleChangeSchedule = (newId: string) => {
  const index = schedules.value.findIndex((s) => s.id === newId)
  if (index === -1) return
  scheduleIndex.value = index
}

const scheduleData: ComputedRef<ScheduleData | undefined> = computed(() => {
  if (schedules.value.length === 0) return undefined
  const schedule = schedules.value[scheduleIndex.value]
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

  const startFirst = parseDate(timeslots[0].startsAt)
  const endLast = parseDate(timeslots[timeslots.length - 1].endsAt)
  const hours = hoursBetween(startFirst, endLast)
  if (startFirst > parseDate(schedule.startsAt)) {
    hours.unshift(getPreviousHour(startFirst))
  }
  if (endLast < parseDate(schedule.endsAt)) {
    hours.push(endLast)
  }

  hours.forEach((hour) => {
    const slot = timeslots.find((t) => Date.parse(t.startsAt) === hour.getTime())
    timeslotData.push({
      start: hour,
      performer: slot?.performer.name ?? '',
      performerUrl: slot?.performer.url ?? '',
      type: slot?.performanceType ?? ''
    })
  })
  return timeslotData
}

onMounted(async () => {
  const response = await schedulesApi.get({ after: new Date().toISOString() })
  if (tryHandleUnsuccessfulResponse(response, toast)) return
  if (response.isSuccess()) {
    schedules.value = response.data().sort((a, b) => Date.parse(a.endsAt) - Date.parse(b.endsAt))
    isLoaded.value = true
  }
})
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
