import type { ValidationRule } from './validationRule'

export interface PropertyValidationState {
  rule: ValidationRule
  isDirty: boolean
  isValid: boolean
  message: string | null
}
