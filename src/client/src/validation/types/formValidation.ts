import { alwaysValid } from './../rules/single/untypedRules'
import type { FormValidationState } from './../types/formValidationState'
import type { PropertyRules } from './../types/propertyRules'
import { invalid, valid, type ValidationResult } from './../types/validationResult'
import type { ValidationRule } from './../types/validationRule'

class FormValidationImplementation<TForm extends object> implements FormValidation<TForm> {
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
      this.propertyState[key] = {
        rule: rule,
        isValid: true,
        message: '',
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

  public setPropertyRule = <TProperty extends keyof TForm>(
    key: TProperty,
    rule: ValidationRule<TForm[TProperty]>
  ) => {
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

  public validateIfDirty = (...keys: (keyof TForm)[]) => {
    if (keys.length == 0) keys = this.getKeys()
    const dirtyKeys = keys.filter((key) => this.propertyState[key].isDirty)
    if (dirtyKeys.length == 0) {
      return true
    }
    return this.validate(...dirtyKeys)
  }

  public isValid = (...keys: (keyof TForm)[]) => {
    if (keys.length == 0) keys = this.getKeys()
    for (let i = 0; i < keys.length; i++) {
      const isKeyValid = this.propertyState[keys[i]].isValid
      if (!isKeyValid) return false
    }
    return true
  }

  public reset = (...keys: (keyof TForm)[]) => {
    if (keys.length == 0) keys = this.getKeys()
    keys.forEach((key) => {
      this.setValidity(key, valid)
      this.propertyState[key].isDirty = false
    })
  }

  public mapApiValidationErrors = (errors: { [key in keyof TForm]: string[] }) => {
    const keys = this.getKeys()
    for (let i = 0; i < keys.length; i++) {
      if (!errors[keys[i]]) continue
      const message = errors[keys[i]][0]
      this.setValidity(keys[i], invalid(message))
    }
  }
}

export interface FormValidation<TForm extends object> {
  message: (key: keyof TForm) => string
  setPropertyRule: <TProperty extends keyof TForm>(
    key: TProperty,
    rule: ValidationRule<TForm[TProperty]>
  ) => void
  validate: (...keys: (keyof TForm)[]) => boolean
  validateIfDirty: (...keys: (keyof TForm)[]) => boolean
  isValid: (...keys: (keyof TForm)[]) => boolean
  reset: (...keys: (keyof TForm)[]) => void
  mapApiValidationErrors: (errors: { [key in keyof TForm]: string[] }) => void
}

export const createFormValidation = <TForm extends object>(
  formState: TForm,
  rules: PropertyRules<TForm>
): FormValidation<TForm> => new FormValidationImplementation<TForm>(formState, rules)
