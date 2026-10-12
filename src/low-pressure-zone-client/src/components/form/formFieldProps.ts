import type { FieldSize } from '@/components/form/fieldSize.ts'

export interface FormFieldProps {
  inputId?: string
  size?: FieldSize
  label?: string
  message?: string
  optional?: boolean
}

export const formFieldPropsDefaults: FormFieldProps = {
  size: 'm',
  optional: false
}
