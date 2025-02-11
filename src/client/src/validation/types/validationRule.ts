import { valid, type ValidationResult } from './validationResult'

export type ValidationRule<T, TForm extends object = never> = (
  arg: T,
  formState?: TForm
) => ValidationResult

/**
 * Assembles an array of rules into a single rule function to be applied on a field being validated. All rules must pass for the field to be valid.
 * @param rules The rules to be applied to the form field value
 * @returns A new `ValidationRule` from the `rules` specified
 */
export const combineRules = <TProperty, TForm extends object = never>(
  ...rules: ValidationRule<TProperty, TForm>[]
): ValidationRule<TProperty, TForm> => {
  return (arg: TProperty, formState?: TForm) => {
    return rules.reduce((result: ValidationResult, rule: ValidationRule<TProperty, TForm>) => {
      if (!result.isValid) {
        return result
      }
      return rule(arg, formState)
    }, valid)
  }
}
