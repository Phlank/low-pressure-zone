import { valid, type ValidationResult } from './validationResult'

export type ValidationRule = <T>(arg: T) => ValidationResult

/**
 * Assembles an array of rules into a single rule function to be applied on a field being validated. All rules must pass for the field to be valid.
 * @param rules The rules to be applied to the form field value
 * @returns A new `ValidationRule` from the `rules` specified
 */
export const combineRules = (...rules: ValidationRule[]): ValidationRule => {
  return <TProperty>(arg: TProperty) => {
    return rules.reduce((result: ValidationResult, rule: (arg: TProperty) => ValidationResult) => {
      if (!result.isValid) {
        return result
      }
      return rule(arg)
    }, valid)
  }
}
