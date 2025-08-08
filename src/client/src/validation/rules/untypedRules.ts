import { invalid, valid } from '@/validation/types/validationResult'
import type { ValidationRule } from '@/validation/types/validationRule.ts'

export const alwaysValid = () => () => valid

export const inArray = (arr: unknown[], msg: string) => (arg: unknown) => {
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
