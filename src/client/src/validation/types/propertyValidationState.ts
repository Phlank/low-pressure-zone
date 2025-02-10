import type { ValidationRule } from './validationRule'

export interface PropertyValidationState<TForm extends object, TProperty extends keyof TForm> {
  rule: ValidationRule<TForm[TProperty]>
  isDirty: boolean
  isValid: boolean
  message: string
}
