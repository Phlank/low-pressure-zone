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
