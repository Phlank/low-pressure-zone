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

export const formatTimeslot = (date: Date) => formatDate(date, 'h:mm A')

export const getDuration = (start: Date | string, end: Date | string) => {
  const timeA = typeof start === 'string' ? parseDate(start).getTime() : start.getTime()
  const timeB = typeof end === 'string' ? parseDate(end).getTime() : end.getTime()
  return Math.abs(timeB - timeA)
}

export const formatDuration = (timechangeMs: number) => {
  const seconds = timechangeMs / 1000
  const minutes = seconds / 60
  const hours = minutes / 60

  let result = ''
  if (hours > 1) result += hours.toFixed(0) + 'h '
  result += (minutes % 60).toFixed(0) + 'm '
  result += (seconds % 60).toFixed(0) + 's'
  return result
}
