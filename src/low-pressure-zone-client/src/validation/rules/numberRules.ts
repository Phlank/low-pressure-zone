import { invalid, valid, type ValidationResult } from '@/validation/types/validationResult.ts'

export const withinRange = (min: number, max: number, msg?: string) => (value?: number): ValidationResult => {
  if (value === undefined) return valid
  if (value >= min && value <= max) return valid
  return invalid(msg ?? `Range is ${min} to ${max}`)
}
