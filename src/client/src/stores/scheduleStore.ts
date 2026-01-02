import { defineStore } from 'pinia'
import { computed, type Ref, ref } from 'vue'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import timeslotsApi from '@/api/resources/timeslotsApi.ts'
import { addDays, compareAsc, getTime } from 'date-fns'
import { useRefresh } from '@/composables/useRefresh.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFunctions.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useToast } from 'primevue'
import { addChronologically, getEntityMap, removeEntity } from '@/utils/arrayUtils.ts'
import {showCreateSuccessToast, showDeleteSuccessToast, showEditSuccessToast } from '@/utils/toastUtils.ts'
import { parseDate } from '@/utils/dateUtils.ts'

const DEFAULT_SCHEDULE_DAY_RANGE = 30

export const useScheduleStore = defineStore('scheduleStore', () => {
  const schedules: Ref<ScheduleResponse[]> = ref([])
  const schedulesMap: Ref<Partial<Record<string, ScheduleResponse>>> = ref({})
  const toast = useToast()
  const { getCommunityById } = useCommunityStore()

  const { isLoading } = useRefresh(
    schedulesApi.get,
    (data) => {
      schedules.value = data.sort((a, b) => compareAsc(a.endsAt, b.endsAt))
      schedulesMap.value = getEntityMap(data)
    },
    {
      params: {
        after: addDays(new Date(), -DEFAULT_SCHEDULE_DAY_RANGE).toISOString(),
        before: addDays(new Date(), DEFAULT_SCHEDULE_DAY_RANGE).toISOString()
      }
    }
  )

  const getSchedules = computed(() => schedules.value)

  const upcomingSchedules = computed(() =>
    schedules.value.filter((schedule) => getTime(schedule.endsAt) > Date.now())
  )

  const pastSchedules = computed(() => {
    return schedules.value.filter((schedule) => getTime(schedule.endsAt) <= Date.now()).reverse()
  })

  const nextSchedule = computed(() => {
    schedules.value.sort((a, b) => compareAsc(a.endsAt, b.endsAt))
    return schedules.value.find((schedule) => getTime(schedule.endsAt) > Date.now())
  })

  const createSchedule = useCreatePersistentItemFn(
    schedulesApi.post,
    (id, form) => {
      const entity: ScheduleResponse = {
        id,
        startsAt: form.startsAt,
        endsAt: form.endsAt,
        description: form.description,
        timeslots: [],
        community: getCommunityById(form.communityId)!,
        isDeletable: true,
        isEditable: true,
        isTimeslotCreationAllowed: true
      }
      addChronologically(schedules.value, entity, (schedule) => schedule.startsAt)
      schedulesMap.value[id] = entity
      showCreateSuccessToast(toast, 'Schedule', parseDate(form.startsAt).toLocaleString())
    },
    toast
  )

  const updateSchedule = useUpdatePersistentItemFn(
    schedules,
    schedulesApi.put,
    (form, entity) => {
      entity.startsAt = form.startsAt
      entity.endsAt = form.endsAt
      entity.description = form.description
      entity.community = getCommunityById(form.communityId)!
      schedules.value.sort((a, b) => compareAsc(a.startsAt, b.startsAt))
      showEditSuccessToast(toast, 'Schedule', parseDate(form.startsAt).toLocaleString())
    },
    toast
  )

  const removeSchedule = useRemovePersistentItemFn(
    schedules,
    schedulesApi.delete,
    (entity) => {
      removeEntity(schedules.value, entity.id)
      schedulesMap.value[entity.id] = undefined
      showDeleteSuccessToast(toast, 'Schedule', parseDate(entity.startsAt).toLocaleString())
    },
    toast
  )

  const reloadTimeslotsAsync = async (scheduleId: string): Promise<void> => {
    if (schedulesMap.value[scheduleId] === undefined) return
    const response = await timeslotsApi.get(scheduleId)
    if (!response.isSuccess()) return
    schedulesMap.value[scheduleId].timeslots = response.data()
  }

  return {
    isLoading,
    nextSchedule,
    schedules: getSchedules,
    upcomingSchedules,
    pastSchedules,
    reloadTimeslotsAsync,
    createSchedule: createSchedule,
    removeSchedule,
    updateSchedule
  }
})
