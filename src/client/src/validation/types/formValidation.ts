import type { ErrorMessageDictionary } from '@/api/apiResponse'
import { ref } from 'vue'
import { alwaysValid } from '../rules/untypedRules'
import type { FormValidationState } from './../types/formValidationState'
import type { PropertyRules } from './../types/propertyRules'
import { invalid, valid, type ValidationResult } from './../types/validationResult'
import type { ValidationRule } from './../types/validationRule'

class FormValidationImplementation<TForm extends object> implements FormValidation<TForm> {
  private readonly formState: TForm
  private readonly propertyState: FormValidationState<TForm>

  constructor(formState: TForm, rules: PropertyRules<TForm>) {
    this.formState = formState
    this.propertyState = {} as FormValidationState<TForm>
    const formKeys = this.getKeys()
    formKeys.forEach((key) => {
      this.propertyState[key] = {
        rule: rules[key] ?? alwaysValid,
        isValid: ref(true),
        message: ref(''),
        isDirty: false
      }
    })
  }

  public message = (key: keyof TForm) => {
    return this.propertyState[key].message.value
  }

  public setPropertyRule = <TProperty extends keyof TForm>(
    key: TProperty,
    rule: ValidationRule<TForm[TProperty]>
  ) => {
    this.propertyState[key].rule = rule
  }

  public validate = (...keys: (keyof TForm)[]) => {
    if (keys.length === 0) {
      keys = this.getKeys()
    }
    keys.forEach((key) => {
      this.setValidity(key, this.propertyState[key].rule(this.formState[key]))
    })
    return this.isValid(...keys)
  }

  public validateIfDirty = (...keys: (keyof TForm)[]) => {
    if (keys.length === 0) keys = this.getKeys()
    const dirtyKeys = keys.filter((key) => this.propertyState[key].isDirty)
    if (dirtyKeys.length === 0) {
      return true
    }
    return this.validate(...dirtyKeys)
  }

  public isValid = (...keys: (keyof TForm)[]) => {
    if (keys.length === 0) keys = this.getKeys()
    for (const key of keys) {
      const isKeyValid = this.propertyState[key].isValid.value
      if (isKeyValid === false) return false
    }
    return true
  }

  public reset = (...keys: (keyof TForm)[]) => {
    if (keys.length === 0) keys = this.getKeys()
    keys.forEach((key) => {
      this.setValidity(key, valid)
      this.propertyState[key].isDirty = false
    })
  }

  public mapApiValidationErrors = <TRequest extends object>(
    errors: ErrorMessageDictionary<TRequest>
  ) => {
    const keys = this.getKeys()
    keys.forEach((key) => {
      if (errors[key as unknown as keyof TRequest] === undefined) return
      const message = errors[key as unknown as keyof TRequest]!.join(' | ')
      this.setValidity(key, invalid(message))
    })
  }

  private setValidity(key: keyof TForm, value: ValidationResult): void {
    const state = this.propertyState[key]
    state.isValid.value = value.isValid
    state.message.value = value.message
    if (!state.isDirty && !value.isValid) {
      state.isDirty = true
    }
  }

  private getKeys(): (keyof TForm)[] {
    return Object.keys(this.formState) as (keyof TForm)[]
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
  mapApiValidationErrors: <TRequest extends object>(
    errors: ErrorMessageDictionary<TRequest>
  ) => void
}

export const createFormValidation = <TForm extends object>(
  formState: TForm,
  rules: PropertyRules<TForm>
): FormValidation<TForm> => new FormValidationImplementation<TForm>(formState, rules)
