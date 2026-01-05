import { defineStore } from 'pinia'
import { computed, type ComputedRef, type Ref, ref, watch } from 'vue'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import timeslotsApi, { type TimeslotResponse } from '@/api/resources/timeslotsApi.ts'
import { addDays, compareAsc, getTime } from 'date-fns'
import { useRefresh } from '@/composables/useRefresh.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFns.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useToast } from 'primevue'
import { addChronologically, getEntity, getEntityMap, removeEntity } from '@/utils/arrayUtils.ts'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils.ts'
import { parseDate } from '@/utils/dateUtils.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { useAuthStore } from '@/stores/authStore.ts'

const DEFAULT_SCHEDULE_DAY_RANGE = 30

export const useScheduleStore = defineStore('scheduleStore', () => {
  const schedules: Ref<ScheduleResponse[]> = ref([])
  const schedulesMap: Ref<Partial<Record<string, ScheduleResponse>>> = ref({})
  const timeslots: ComputedRef<TimeslotResponse[]> = computed(() =>
    schedules.value.flatMap((schedule) => schedule.timeslots)
  )
  const toast = useToast()
  const performers = usePerformerStore()
  const auth = useAuthStore()
  const { getCommunityById } = useCommunityStore()

  const { isLoading, refresh } = useRefresh(
    schedulesApi.get,
    (data) => {
      schedules.value = [...data].sort((a, b) => compareAsc(a.endsAt, b.endsAt))
      schedulesMap.value = getEntityMap(data)
    },
    {
      params: {
        after: addDays(new Date(), -DEFAULT_SCHEDULE_DAY_RANGE).toISOString(),
        before: addDays(new Date(), DEFAULT_SCHEDULE_DAY_RANGE).toISOString()
      }
    }
  )
  watch(
    () => auth.isLoggedIn,
    async (newVal) => {
      if (newVal) await refresh()
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

  const getScheduleById = (id: string): ScheduleResponse | undefined => {
    return schedulesMap.value[id]
  }

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

  const getTimeslotById = (id: string) => getEntity(timeslots.value, id)

  const createTimeslot = useCreatePersistentItemFn(
    timeslotsApi.post,
    (id, form) => {
      const schedule = getEntity(schedules.value, form.scheduleId)
      const performer = performers.getById(form.performerId)
      if (!schedule || !performer) throw new Error('Schedule or performer not found')
      const entity = {
        id,
        scheduleId: form.scheduleId,
        performer: performer,
        performanceType: form.performanceType,
        name: form.name,
        startsAt: form.startsAt,
        endsAt: form.endsAt,
        isEditable: true,
        isDeletable: true,
        uploadedFileName: form.file?.name ?? null
      }
      addChronologically(schedule.timeslots, entity, (timeslot) => timeslot.startsAt)
    },
    toast
  )

  const updateTimeslot = useUpdatePersistentItemFn(
    timeslots,
    timeslotsApi.put,
    (form, entity) => {
      entity.performanceType = form.performanceType
      entity.name = form.name
      entity.startsAt = form.startsAt
      entity.endsAt = form.endsAt
      if (form.replaceMedia && form.file) {
        entity.uploadedFileName = form.file.name
      }
      entity.performer = performers.getById(form.performerId)!
      showEditSuccessToast(toast, 'Timeslot', parseDate(entity.startsAt).toLocaleString())
    },
    toast
  )

  const removeTimeslot = useRemovePersistentItemFn(
    timeslots,
    timeslotsApi.delete,
    (entity) => {
      const schedule = schedulesMap.value[entity.scheduleId]
      if (!schedule) return
      removeEntity(schedule.timeslots, entity.id)
      showDeleteSuccessToast(toast, 'Timeslot', parseDate(entity.startsAt).toLocaleString())
    },
    toast
  )

  return {
    isLoading,
    refresh,
    nextSchedule,
    schedules: getSchedules,
    upcomingSchedules,
    pastSchedules,
    getScheduleById,
    createSchedule,
    removeSchedule,
    updateSchedule,
    getTimeslotById,
    createTimeslot,
    updateTimeslot,
    removeTimeslot
  }
})
