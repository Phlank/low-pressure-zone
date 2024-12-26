import type { ValidationRule } from './validationRule'

export type PropertyRules<TForm extends object> = {
  [Key in keyof TForm]: ValidationRule
}
