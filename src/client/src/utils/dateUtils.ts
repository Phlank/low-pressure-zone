export const setToHour = (date: Date) => {
  date.setMinutes(0)
  date.setSeconds(0)
  date.setMilliseconds(0)
}

export const setToNextHour = (date: Date) => {
  date.setHours(date.getHours() + 1)
  setToHour(date)
}

export const isHour = (date: Date) =>
  date.getMinutes() == 0 && date.getSeconds() == 0 && date.getMilliseconds() == 0

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
