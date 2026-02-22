import { defineStore } from 'pinia'
import { computed, type ComputedRef, type Ref, ref, watch } from 'vue'
import schedulesApi, {
  type ScheduleRequest,
  type ScheduleResponse
} from '@/api/resources/schedulesApi.ts'
import timeslotsApi, {
  type TimeslotRequest,
  type TimeslotResponse
} from '@/api/resources/timeslotsApi.ts'
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
import soundclashApi, {
  type SoundclashRequest,
  type SoundclashResponse
} from '@/api/resources/soundclashApi.ts'
import { scheduleTypes } from '@/constants/scheduleTypes.ts'

const DEFAULT_SCHEDULE_DAY_RANGE = 30

export const useScheduleStore = defineStore('scheduleStore', () => {
  const schedules: Ref<ScheduleResponse[]> = ref([])
  const schedulesMap: Ref<Partial<Record<string, ScheduleResponse>>> = ref({})
  const timeslots: ComputedRef<TimeslotResponse[]> = computed(() =>
    schedules.value.flatMap((schedule) => schedule.timeslots)
  )
  const soundclashes: ComputedRef<SoundclashResponse[]> = computed(() =>
    schedules.value.flatMap((schedule) => schedule.soundclashes)
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
        after: addDays(new Date(), -DEFAULT_SCHEDULE_DAY_RANGE).toISOString()
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

  const createSchedule = useCreatePersistentItemFn<ScheduleRequest>(
    schedulesApi.post,
    (id, form) => {
      const entity: ScheduleResponse = {
        id,
        type: form.type,
        startsAt: form.startsAt,
        endsAt: form.endsAt,
        name: form.name,
        description: form.description,
        timeslots: [],
        soundclashes: [],
        community: getCommunityById(form.communityId)!,
        isDeletable: true,
        isEditable: true,
        isOrganizersOnly: form.isOrganizersOnly,
        isTimeslotCreationAllowed: form.type === scheduleTypes.Hourly,
        isSoundclashCreationAllowed: form.type === scheduleTypes.Soundclash
      }
      addChronologically(schedules.value, entity, (schedule) => schedule.startsAt)
      schedulesMap.value[id] = entity
      showCreateSuccessToast(toast, 'Schedule', parseDate(form.startsAt).toLocaleString())
    },
    toast
  )

  const updateSchedule = useUpdatePersistentItemFn<ScheduleRequest, ScheduleResponse>(
    schedules,
    schedulesApi.put,
    (form, entity) => {
      entity.type = form.type
      entity.startsAt = form.startsAt
      entity.endsAt = form.endsAt
      entity.name = form.name
      entity.description = form.description
      entity.isOrganizersOnly = form.isOrganizersOnly
      entity.community = getCommunityById(form.communityId)!
      schedules.value.sort((a, b) => compareAsc(a.startsAt, b.startsAt))
      showEditSuccessToast(toast, 'Schedule', parseDate(form.startsAt).toLocaleString())
    },
    toast
  )

  const removeSchedule = useRemovePersistentItemFn<ScheduleResponse>(
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

  const createTimeslot = useCreatePersistentItemFn<TimeslotRequest>(
    timeslotsApi.post,
    (id, form) => {
      const schedule = getEntity(schedules.value, form.scheduleId)
      const performer = performers.getById(form.performerId)
      if (!schedule || !performer) throw new Error('Schedule or performer not found')
      const entity: TimeslotResponse = {
        id,
        scheduleId: form.scheduleId,
        performer: performer,
        performanceType: form.performanceType,
        subtitle: form.subtitle,
        startsAt: form.startsAt,
        endsAt: form.endsAt,
        isEditable: true,
        isDeletable: true,
        uploadedFileName: form.file?.name ?? null
      }
      addChronologically(schedule.timeslots, entity, (timeslot) => timeslot.startsAt)
      showCreateSuccessToast(toast, 'Timeslot', parseDate(entity.startsAt).toLocaleString())
    },
    toast
  )

  const updateTimeslot = useUpdatePersistentItemFn<TimeslotRequest, TimeslotResponse>(
    timeslots,
    timeslotsApi.put,
    (form, entity) => {
      entity.performanceType = form.performanceType
      entity.subtitle = form.subtitle
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

  const removeTimeslot = useRemovePersistentItemFn<TimeslotResponse>(
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

  const getSoundclashById = (id: string) => getEntity(soundclashes.value, id)

  const createSoundclash = useCreatePersistentItemFn<SoundclashRequest>(
    soundclashApi.post,
    (id, request) => {
      const entity: SoundclashResponse = {
        id: id,
        scheduleId: request.scheduleId,
        performerOne: performers.getById(request.performerOneId)!,
        performerTwo: performers.getById(request.performerTwoId)!,
        roundOne: request.roundOne,
        roundTwo: request.roundTwo,
        roundThree: request.roundThree,
        startsAt: request.startsAt,
        endsAt: request.endsAt,
        isEditable: true,
        isDeletable: true
      }
      addChronologically(
        getScheduleById(entity.scheduleId)!.soundclashes,
        entity,
        (soundclash) => soundclash.startsAt
      )
      showCreateSuccessToast(
        toast,
        'Soundclash',
        `${entity.performerOne.name} vs. ${entity.performerTwo.name}`
      )
    },
    toast
  )

  const updateSoundclash = useUpdatePersistentItemFn<SoundclashRequest, SoundclashResponse>(
    soundclashes,
    soundclashApi.put,
    (form, entity) => {
      entity.scheduleId = form.scheduleId
      entity.performerOne = performers.getById(form.performerOneId)!
      entity.performerTwo = performers.getById(form.performerTwoId)!
      entity.roundOne = form.roundOne
      entity.roundTwo = form.roundTwo
      entity.roundThree = form.roundThree
      entity.startsAt = form.startsAt
      entity.endsAt = form.endsAt
      showEditSuccessToast(
        toast,
        'Soundclash',
        `${entity.performerOne.name} vs. ${entity.performerTwo.name}`
      )
    },
    toast
  )

  const deleteSoundclash = useRemovePersistentItemFn<SoundclashResponse>(
    soundclashes,
    soundclashApi.delete,
    (entity) => {
      removeEntity(getScheduleById(entity.scheduleId)?.soundclashes ?? [], entity.id)
      showDeleteSuccessToast(
        toast,
        'Soundclash',
        `${entity.performerOne.name} vs. ${entity.performerTwo.name}`
      )
    }
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
    removeTimeslot,
    getSoundclashById,
    createSoundclash,
    updateSoundclash,
    deleteSoundclash
  }
})
