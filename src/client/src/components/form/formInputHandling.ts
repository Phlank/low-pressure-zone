import type { FormValidation } from '@/validation/types/formValidation'

export const createUpdateHandler =
  <TForm extends object, TProperty extends keyof TForm>(
    form: TForm,
    validation: FormValidation<TForm>,
    field: TProperty,
    defaultValue: TForm[TProperty]
  ) =>
  (value?: TForm[TProperty]) => {
    if (value == undefined) value = defaultValue
    form[field] = value
    validation.validateIfDirty(field)
  }
