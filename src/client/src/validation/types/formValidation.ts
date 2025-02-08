import { alwaysValid } from './../rules/single/untypedRules'
import type { FormValidationState } from './../types/formValidationState'
import type { PropertyRules } from './../types/propertyRules'
import { valid, type ValidationResult } from './../types/validationResult'
import type { ValidationRule } from './../types/validationRule'

export class FormValidation<TForm extends object> {
  private formState: TForm
  private propertyState: FormValidationState<TForm>

  constructor(formState: TForm, rules: PropertyRules<TForm>) {
    this.formState = formState
    this.propertyState = {} as FormValidationState<TForm>
    const formKeys = this.getKeys()
    formKeys.forEach((key) => {
      let rule = rules[key]
      if (!rule) {
        rule = alwaysValid
      }
      this.propertyState[key as keyof TForm] = {
        rule: rule,
        isValid: true,
        message: null,
        isDirty: false
      }
    })
  }

  private setValidity(key: keyof TForm, value: ValidationResult): void {
    const state = this.propertyState[key]
    state.isValid = value.isValid
    state.message = value.message
    if (!state.isDirty && !value.isValid) {
      state.isDirty = true
    }
  }

  private getKeys(): (keyof TForm)[] {
    return Object.keys(this.formState) as (keyof TForm)[]
  }

  public message = (key: keyof TForm) => {
    return this.propertyState[key].message
  }

  public setPropertyRule = <T>(key: keyof TForm, rule: ValidationRule<T>) => {
    this.propertyState[key].rule = rule
  }

  public validate = (...keys: (keyof TForm)[]) => {
    if (keys.length == 0) {
      keys = this.getKeys()
    }
    keys.forEach((key) => {
      const rule = this.propertyState[key].rule
      const result = rule ? rule(this.formState[key]) : valid
      this.setValidity(key, result)
    })
    return this.isValid(...keys)
  }

  validateIfDirty(...keys: (keyof TForm)[]): boolean {
    if (keys.length == 0) keys = this.getKeys()
    const dirtyKeys = keys.filter((key) => this.propertyState[key].isDirty)
    if (dirtyKeys.length == 0) {
      return true
    }
    return this.validate(...dirtyKeys)
  }

  isValid(...keys: (keyof TForm)[]): boolean {
    if (keys.length == 0) keys = this.getKeys()
    for (let i = 0; i < keys.length; i++) {
      const isKeyValid = this.propertyState[keys[i]].isValid
      if (!isKeyValid) return false
    }
    return true
  }

  reset(...keys: (keyof TForm)[]) {
    if (keys.length == 0) keys = this.getKeys()
    keys.forEach((key) => {
      this.setValidity(key, valid)
    })
  }
}
