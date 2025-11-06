import { formatDate } from '@vueuse/core'

export const parseDate = (dateString: string) => new Date(Date.parse(dateString))

export const parseTime = (dateString: string) => parseDate(dateString).getTime()

export const compareDateStrings = (a: string, b: string) => parseTime(b) - parseTime(a)

export const setToHour = (date: Date) => {
  date.setMinutes(0)
  date.setSeconds(0)
  date.setMilliseconds(0)
}

export const getNextHour = (date: Date) => {
  const newDate = new Date(date)
  setToNextHour(newDate)
  return newDate
}

export const setToNextHour = (date: Date) => {
  date.setHours(date.getHours() + 1)
  setToHour(date)
}

export const getPreviousHour = (date: Date) => {
  const newDate = new Date(date)
  setToPreviousHour(newDate)
  return newDate
}

export const setToPreviousHour = (date: Date) => {
  setToHour(date)
  date.setHours(date.getHours() - 1)
}

export const isHour = (date: Date) =>
  date.getMinutes() === 0 && date.getSeconds() === 0 && date.getMilliseconds() === 0

export const hoursBetween = (start: Date, end: Date) => {
  const out: Date[] = []
  const iterating = new Date(start.getTime())
  while (iterating.getTime() < end.getTime()) {
    if (isHour(iterating)) {
      out.push(new Date(iterating.getTime()))
    }
    setToNextHour(iterating)
  }
  return out
}

export const minimumDate = (...dates: Date[]) => {
  let minimum = dates[0].getTime()
  for (let i = 1; i < dates.length; i++) {
    const currentTime = dates[i].getTime()
    if (currentTime < minimum) {
      minimum = currentTime
    }
  }
  return new Date(minimum)
}

const readableFormat = 'h:mm A'
export const formatReadableTime = (date: Date | string) => {
  if (typeof date === 'string') {
    return formatDate(parseDate(date), readableFormat)
  }
  return formatDate(date, readableFormat)
}

export const getDuration = (start: Date | string, end: Date | string) => {
  const timeA = typeof start === 'string' ? parseDate(start).getTime() : start.getTime()
  const timeB = typeof end === 'string' ? parseDate(end).getTime() : end.getTime()
  return Math.abs(timeB - timeA)
}

export const formatDurationTimestamp = (durationMs: number) => {
  const seconds = Math.floor(durationMs / 1000)
  const minutes = Math.floor(seconds / 60)
  const hours = Math.floor(minutes / 60)
  return `${Math.floor(hours).toFixed(0)}:${formatTwoDigits(minutes % 60)}:${formatTwoDigits(seconds % 60)}`
}

export const formatDurationOption = (durationMinutes: number) => {
  const minutes = durationMinutes % 60
  const hours = Math.floor(durationMinutes / 60)
  let output = `${hours.toFixed(0)}h`
  if (minutes !== 0) {
    output += ` ${minutes.toFixed(0)}m`
  }
  return output
}

const formatTwoDigits = (value: number) => ('0' + value.toFixed(0)).slice(-2)

export const formatForFilename = (date: Date) =>
  `${date.getFullYear()}-${date.getMonth()}-${date.getDate()}_${date.getHours()}-${date.getMinutes()}-${date.getSeconds()}`

export const isDateInTimeslot = (date: Date, timespan: { startsAt: string, endsAt: string }) => {
  const time = date.getTime()
  const startsAt = parseTime(timespan.startsAt)
  const endsAt = parseTime(timespan.endsAt)
  return time >= startsAt && time < endsAt
}
