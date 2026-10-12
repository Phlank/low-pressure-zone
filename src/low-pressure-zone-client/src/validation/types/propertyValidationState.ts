import type { Ref } from 'vue'
import type { ValidationRule } from './validationRule'

export interface PropertyValidationState<TForm extends object, TProperty extends keyof TForm> {
  rule: ValidationRule<TForm[TProperty]>
  isDirty: boolean
  isValid: Ref<boolean, boolean>
  message: Ref<string, string>
}
