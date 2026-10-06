import { invalid, valid } from '@/validation/types/validationResult.ts'
import type { ValidationRule } from '@/validation/types/validationRule.ts'

export const notEmptyArray = (msg?: string) => <T>(value?: T[]) => {
  if (!value) return valid
  if (value.length === 0) {
    return invalid(msg ?? 'Cannot be empty')
  }
  return valid
}

export const arrayLength = (min: number, max: number, msg?: string) => <T>(value?: T[]) => {
  if (!value) return valid
  if (value.length < min || value.length > max) {
    return invalid(msg ?? `Length must be between ${min} and ${max}`)
  }
  return valid
}

export const each = <T>(rule: ValidationRule<T>) => (arg?: T[]) => {
  if (!arg) return valid
  for (const item of arg) {
    const result = rule(item)
    if (!result.isValid) return result
  }
  return valid
}
