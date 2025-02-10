import { invalid, valid } from '@/validation/types/validationResult'
import type { ValidationRule } from '@/validation/types/validationRule'

export const required =
  (msg?: string): ValidationRule<string> =>
  (arg) => {
    if (arg.trim().length == 0) return invalid(msg ?? 'Required field')
    return valid
  }

export const url =
  (msg?: string): ValidationRule<string> =>
  (arg) => {
    if (arg.trim().length == 0) return valid
    if (URL.parse(arg)) return valid
    return invalid(msg ?? 'Invalid URL')
  }
