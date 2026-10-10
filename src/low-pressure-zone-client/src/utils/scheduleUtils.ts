import { parseDate, parseTime, timesBetween } from '@/utils/dateUtils.ts'
import type { ScheduleResponse, SlotResponse } from '@/api/resources/schedulesApi.ts'

export const getPublicSlotHours = (schedule: ScheduleResponse): Date[] => {
  if (schedule.slots.length === 0) return []
  const startFirst = parseDate(schedule.slots.at(0)!.startsAt)
  const endLast = parseDate(schedule.slots.at(-1)!.endsAt)

  return timesBetween(startFirst, endLast, 60)
}

export const getSlotForTime = (
  schedule: ScheduleResponse,
  time: Date
): SlotResponse | undefined => {
  return schedule.slots?.find((slot) => {
    return time.getTime() < parseTime(slot.endsAt) && time.getTime() >= parseTime(slot.startsAt)
  })
}
