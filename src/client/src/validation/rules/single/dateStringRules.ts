import { MS_PER_MINUTE } from '@/constants/times'
import { invalid, valid } from '@/validation/types/validationResult'
import type { Ref } from 'vue'

export const withinRangeOf =
  (other: Ref<string>, minMinutes: number, maxMinutes: number, msg?: string) => (arg: string) => {
    const otherTime = Date.parse(other.value)
    const thisTime = Date.parse(arg)
    if (!otherTime || !thisTime) return valid // Don't unintentionally enforce required validation
    if (
      otherTime + MS_PER_MINUTE * minMinutes <= thisTime &&
      otherTime + MS_PER_MINUTE * maxMinutes >= thisTime
    ) {
      return valid
    }
    return invalid(msg ?? 'Out of range')
  }

export const hourOnly = (msg?: string) => (arg: string) => {
  const time = new Date(Date.parse(arg))
  if (!time) return valid // Don't unintentionally enforce required() validation
  if (time.getMinutes() != 0 || time.getSeconds() != 0 || time.getMilliseconds() != 0) {
    return invalid(msg ?? 'Hour intervals only')
  }
  return valid
}
