import { MS_PER_MINUTE } from '@/constants/times'
import { invalid, valid } from '@/validation/types/validationResult'
import type { Ref } from 'vue'

export const withinRangeOf =
  (other: Ref<string>, minMinutes: number, maxMinutes: number, msg?: string) => (arg: string) => {
    console.log(`Validating ${arg} is in range of ${other.value}`)
    const otherTime = Date.parse(other.value)
    const thisTime = Date.parse(arg)
    if (!otherTime || !thisTime) return invalid('Out of range')
    if (
      otherTime + MS_PER_MINUTE * minMinutes <= thisTime &&
      otherTime + MS_PER_MINUTE * maxMinutes >= thisTime
    ) {
      return valid
    }
    return invalid(msg ?? 'Out of range')
  }
