import { defineStore } from 'pinia'
import { computed, type Ref, ref } from 'vue'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import timeslotsApi from '@/api/resources/timeslotsApi.ts'
import { addDays, compareAsc, getTime } from 'date-fns'

export const useScheduleStore = defineStore('scheduleStore', () => {
  const loadedSchedules: Ref<ScheduleResponse[]> = ref([])
  const loadedSchedulesMap: Ref<ScheduleMap> = ref({})

  const defaultScheduleDayRange = 30

  let loadDefaultSchedulesPromise: Promise<void> | undefined = undefined
  const loadDefaultSchedules = async (): Promise<void> => {
    const parameters = {
      after: addDays(new Date(), -defaultScheduleDayRange).toISOString(),
      before: addDays(new Date(), defaultScheduleDayRange).toISOString()
    }
    const response = await schedulesApi.get(parameters)
    if (!response.isSuccess()) return
    loadedSchedules.value = response.data().sort((a, b) => compareAsc(a.endsAt, b.endsAt))
    loadedSchedulesMap.value = mapSchedules(response.data())
  }

  const loadDefaultSchedulesAsync = async (): Promise<void> => {
    loadDefaultSchedulesPromise ??= loadDefaultSchedules()
    return await loadDefaultSchedulesPromise
  }

  const schedules = computed(() => loadedSchedules.value)

  const upcomingSchedules = computed(() =>
    loadedSchedules.value.filter((schedule) => getTime(schedule.endsAt) > Date.now())
  )

  const pastSchedules = computed(() => {
    const schedules = loadedSchedules.value.filter(
      (schedule) => getTime(schedule.endsAt) <= Date.now()
    )
    return [...schedules].reverse()
  })

  const nextSchedule = computed(() => {
    loadedSchedules.value.sort((a, b) => compareAsc(a.endsAt, b.endsAt))
    return loadedSchedules.value.find((schedule) => getTime(schedule.endsAt) > Date.now())
  })

  const addSchedule = (schedule: ScheduleResponse) => {
    loadedSchedulesMap.value[schedule.id] = schedule
    const index = loadedSchedules.value.findIndex(
      (loadedSchedule) => getTime(loadedSchedule.endsAt) > getTime(schedule.endsAt)
    )
    loadedSchedules.value.splice(index, 0, schedule)
  }

  const removeSchedule = (id: string) => {
    const index = loadedSchedules.value.findIndex((schedule) => schedule.id === id)
    if (index > -1) {
      loadedSchedules.value.splice(index, 1)
      loadedSchedulesMap.value[id] = undefined
    }
  }

  const updateScheduleAsync = async (id: string) => {
    const response = await schedulesApi.getById(id)
    if (!response.isSuccess()) return
    const index = loadedSchedules.value.findIndex((schedule) => schedule.id === id)
    if (index > -1) {
      loadedSchedules.value.splice(index, 1, response.data())
      loadedSchedulesMap.value[id] = response.data()
    } else {
      addSchedule(response.data())
    }
  }

  const reloadTimeslotsAsync = async (scheduleId: string): Promise<void> => {
    if (loadedSchedulesMap.value[scheduleId] === undefined) return
    const response = await timeslotsApi.get(scheduleId)
    if (!response.isSuccess()) return
    loadedSchedulesMap.value[scheduleId].timeslots = response.data()
  }

  return {
    nextSchedule,
    schedules,
    upcomingSchedules,
    pastSchedules,
    loadDefaultSchedulesAsync,
    reloadTimeslotsAsync,
    addSchedule,
    removeSchedule,
    updateScheduleAsync
  }
})

type ScheduleMap = { [id: string]: ScheduleResponse | undefined }
const mapSchedules = (schedules: ScheduleResponse[]) => {
  const map: ScheduleMap = {}
  schedules.forEach((schedule) => {
    map[schedule.id] = schedule
  })
  return map
}
