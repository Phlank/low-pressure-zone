import { invalid, valid } from '@/validation/types/validationResult'
import type { ValidationRule } from '@/validation/types/validationRule.ts'

export const alwaysValid = () => () => valid

export const required =
  <T>(msg?: string): ValidationRule<T> =>
  (arg?: T) => {
    if (typeof arg === 'string') {
      if (arg.trim().length === 0) return invalid(msg ?? 'Required field')
    } else if (arg === null || arg === undefined) {
      return invalid(msg ?? 'Required field')
    }
    return valid
  }

export const oneOf =
  <T>(arr: T[], msg: string) =>
  (arg: T) => {
    if (arr.indexOf(arg) > -1) return valid
    return invalid(msg)
  }

export const applyRuleIf = <T, TForm extends object>(
  rule: ValidationRule<T, TForm>,
  condition: () => boolean
) => {
  if (condition()) {
    return rule
  }
  return alwaysValid()
}
