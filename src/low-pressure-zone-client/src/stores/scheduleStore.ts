import { defineStore } from 'pinia'
import { computed, type ComputedRef, type Ref, ref, watch } from 'vue'
import schedulesApi, {
  type ScheduleRequest,
  type ScheduleResponse
} from '@/api/resources/schedulesApi.ts'
import { addDays, addHours, compareAsc, getTime } from 'date-fns'
import { useRefresh } from '@/composables/useRefresh.ts'
import {
  useCreatePersistentItemFn,
  useRemovePersistentItemFn,
  useUpdatePersistentItemFn
} from '@/utils/storeFns.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useToast } from 'openvue'
import { addChronologically, getEntity, getEntityMap, removeEntity } from '@/utils/arrayUtils.ts'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils.ts'
import { parseDate } from '@/utils/dateUtils.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import {
  hourlySlotsApi,
  type HourlySlotResponse,
  type HourlySlotRequest
} from '@/api/resources/hourlySlotsApi'
import clashSlotsApi, {
  type ClashSlotRequest,
  type ClashSlotResponse
} from '@/api/resources/clashSlotsApi.ts'

const DEFAULT_SCHEDULE_DAY_RANGE = 30

export const useScheduleStore = defineStore('scheduleStore', () => {
  const schedules: Ref<ScheduleResponse[]> = ref([])
  const schedulesMap: Ref<Partial<Record<string, ScheduleResponse>>> = ref({})
  const hourlySlots: ComputedRef<HourlySlotResponse[]> = computed(() =>
    schedules.value.flatMap((schedule) => schedule.slots).filter((slot) => slot.type === 'Hourly')
  )
  const clashSlots: ComputedRef<ClashSlotResponse[]> = computed(() =>
    schedules.value.flatMap((schedule) => schedule.slots).filter((slot) => slot.type === 'Clash')
  )
  const toast = useToast()
  const performers = usePerformerStore()
  const communities = useCommunityStore()
  const auth = useAuthStore()

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
        name: form.name,
        description: form.description,
        community: communities.getCommunityById(form.communityId)!,
        startsAt: form.startsAt,
        endsAt: form.endsAt,
        slots: [],
        isDeletable: true,
        isEditable: true,
        isVisibleToPublic: form.isVisibleToPublic,
        isHourlyAllowed: form.isHourlyAllowed,
        isHourlySlotCreationAllowed: form.isHourlyAllowed,
        isClashAllowed: form.isClashAllowed,
        isClashSlotCreationAllowed: form.isClashAllowed
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
      entity.startsAt = form.startsAt
      entity.endsAt = form.endsAt
      entity.name = form.name
      entity.description = form.description
      entity.isVisibleToPublic = form.isVisibleToPublic
      entity.community = communities.getCommunityById(form.communityId)!
      entity.isClashAllowed = form.isClashAllowed
      entity.isHourlyAllowed = form.isHourlyAllowed
      schedules.value.sort((a, b) => compareAsc(a.startsAt, b.startsAt))
      showEditSuccessToast(toast, 'Schedule', parseDate(form.startsAt).toLocaleString())
    },
    toast
  )

  const deleteSchedule = useRemovePersistentItemFn<ScheduleResponse>(
    schedules,
    schedulesApi.delete,
    (entity) => {
      removeEntity(schedules.value, entity.id)
      schedulesMap.value[entity.id] = undefined
      showDeleteSuccessToast(toast, 'Schedule', parseDate(entity.startsAt).toLocaleString())
    },
    toast
  )

  const getHourlySlotById = (id: string) => getEntity(hourlySlots.value, id)

  const createHourlySlot = (
    scheduleId: string) =>
    useCreatePersistentItemFn<HourlySlotRequest>(
      hourlySlotsApi.post(scheduleId),
      (id, form) => {
        const schedule = getEntity(schedules.value, scheduleId)
        const performer = performers.getById(form.performerId)
        if (!schedule || !performer) throw new Error('Schedule or performer not found')
        const entity: HourlySlotResponse = {
          id,
          scheduleId: scheduleId,
          performerId: performer.id,
          subtitle: form.subtitle,
          startsAt: form.startsAt,
          endsAt: addHours(form.startsAt, form.duration).toISOString(),
          duration: form.duration,
          isPrerecorded: !!form.file,
          uploadedFileName: form.file?.name ?? null,
          isEditable: true,
          isDeletable: true,
          type: 'Hourly'
        }
        addChronologically(schedule.slots, entity, (slot) => slot.startsAt)
        showCreateSuccessToast(toast, 'Timeslot', parseDate(entity.startsAt).toLocaleString())
      },
      toast
    )

  const updateHourlySlot = (scheduleId: string) =>
    useUpdatePersistentItemFn<HourlySlotRequest, HourlySlotResponse>(
      hourlySlots,
      hourlySlotsApi.put(scheduleId),
      (form, entity) => {
        entity.subtitle = form.subtitle
        entity.startsAt = form.startsAt
        entity.endsAt = addHours(form.startsAt, form.duration).toISOString()
        entity.duration = form.duration
        if (form.replaceMedia && form.file) {
          entity.uploadedFileName = form.file.name
        }
        entity.performerId = form.performerId
        showEditSuccessToast(toast, 'Hourly Slot', parseDate(entity.startsAt).toLocaleString())
      },
      toast
    )

  const deleteHourlySlot = (slot: HourlySlotResponse) =>
    useRemovePersistentItemFn<HourlySlotResponse>(
      hourlySlots,
      hourlySlotsApi.delete(slot.scheduleId),
      (entity) => {
        const schedule = schedulesMap.value[entity.scheduleId]
        if (!schedule) return
        removeEntity(schedule.slots, entity.id)
        showDeleteSuccessToast(toast, 'Timeslot', parseDate(entity.startsAt).toLocaleString())
      },
      toast
    )(slot.id)

  const getSoundclashById = (id: string) => getEntity(clashSlots.value, id)

  const createClashSlot = (scheduleId: string) =>
    useCreatePersistentItemFn<ClashSlotRequest>(
      clashSlotsApi.post(scheduleId),
      (id, request) => {
        const entity: ClashSlotResponse = {
          id: id,
          scheduleId: scheduleId,
          performerOneId: request.performerOneId,
          performerTwoId: request.performerTwoId,
          rounds: request.rounds,
          startsAt: request.startsAt,
          duration: request.duration,
          endsAt: addHours(request.startsAt, request.duration).toISOString(),
          isEditable: true,
          isDeletable: true,
          type: 'Clash'
        }
        addChronologically(
          getScheduleById(entity.scheduleId)!.slots,
          entity,
          (soundclash) => soundclash.startsAt
        )
        showCreateSuccessToast(
          toast,
          'Clash Slot',
          `${performers.getById(entity.performerOneId)?.name} vs. ${performers.getById(entity.performerTwoId)?.name}`
        )
      },
      toast
    )

  const updateClashSlot = (scheduleId: string) =>
    useUpdatePersistentItemFn<ClashSlotRequest, ClashSlotResponse>(
      clashSlots,
      clashSlotsApi.put(scheduleId),
      (form, entity) => {
        entity.scheduleId = scheduleId
        entity.performerOneId = form.performerOneId
        entity.performerTwoId = form.performerTwoId
        entity.rounds = form.rounds
        entity.startsAt = form.startsAt
        entity.endsAt = addHours(form.startsAt, form.duration).toISOString()
        entity.duration = form.duration
        showEditSuccessToast(
          toast,
          'Soundclash',
          `${performers.getById(entity.performerOneId)?.name} vs. ${performers.getById(entity.performerTwoId)?.name}`
        )
      },
      toast
    )

  const deleteClashSlot = (scheduleId: string) =>
    useRemovePersistentItemFn<ClashSlotResponse>(
      clashSlots,
      clashSlotsApi.delete(scheduleId),
      (entity) => {
        removeEntity(getScheduleById(entity.scheduleId)!.slots, entity.id)
        showDeleteSuccessToast(
          toast,
          'Soundclash',
          `${performers.getById(entity.performerOneId)?.name} vs. ${performers.getById(entity.performerTwoId)?.name}`
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
    deleteSchedule,
    updateSchedule,
    getHourlySlotById,
    createHourlySlot,
    updateHourlySlot,
    deleteHourlySlot,
    getSoundclashById,
    createClashSlot,
    updateClashSlot,
    deleteClashSlot
  }
})
