import { scheduleTypes } from '@/constants/scheduleTypes.ts'
import { parseDate, parseTime, timesBetween } from '@/utils/dateUtils.ts'
import { addHours } from 'date-fns'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import type { TimeslotResponse } from '@/api/resources/timeslotsApi.ts'
import type { SoundclashResponse } from '@/api/resources/soundclashApi.ts'

export const getPublicSlotHours = (schedule: ScheduleResponse): Date[] => {
  const isHourly = schedule.type === scheduleTypes.Hourly

  let startFirst: Date
  if (schedule.soundclashes?.length ?? 0 > 0)
    startFirst = parseDate(schedule.soundclashes[0]!.startsAt)
  else if (schedule.timeslots?.length ?? 0 > 0)
    startFirst = parseDate(schedule.timeslots[0]!.startsAt)
  else startFirst = parseDate(schedule.startsAt)

  let endLast: Date
  if (schedule.soundclashes?.length ?? 0 > 0)
    endLast = parseDate(schedule.soundclashes[0]!.startsAt)
  else if (schedule.timeslots?.length ?? 0 > 0)
    endLast = parseDate(schedule.timeslots[schedule.timeslots.length - 1]!.endsAt)
  else endLast = parseDate(schedule.endsAt)

  const hours = timesBetween(startFirst, endLast, isHourly ? 60 : 120)
  if (startFirst > parseDate(schedule.startsAt))
    hours.unshift(addHours(startFirst, isHourly ? -1 : -2))
  if (endLast < parseDate(schedule.endsAt)) hours.push(addHours(endLast, isHourly ? 1 : 2))
  return hours
}

export const getSlotForTime = (
  schedule: ScheduleResponse,
  time: Date
): TimeslotResponse | SoundclashResponse | undefined => {
  if (schedule.type === scheduleTypes.Hourly) {
    return schedule.timeslots?.find((timeslot) => {
      return (
        time.getTime() < parseTime(timeslot.endsAt) &&
        time.getTime() >= parseTime(timeslot.startsAt)
      )
    })
  } else {
    return schedule.soundclashes?.find((soundclash) => {
      return (
        time.getTime() < parseTime(soundclash.endsAt) &&
        time.getTime() >= parseTime(soundclash.startsAt)
      )
    })
  }
}
