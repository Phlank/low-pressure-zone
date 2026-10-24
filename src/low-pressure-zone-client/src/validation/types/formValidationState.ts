import type { PropertyValidationState } from './propertyValidationState'

export type FormValidationState<TForm extends object> = {
  [Key in keyof TForm]: PropertyValidationState<TForm, Key>
}
