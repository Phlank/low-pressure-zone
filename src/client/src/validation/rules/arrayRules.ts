import { invalid, valid } from '@/validation/types/validationResult.ts'

export const notEmpty = (msg?: string) => <T>(value?: T[]) => {
  console.log('validating')
  if (!value) return valid
  if (value.length === 0) {
    return invalid(msg ?? 'Cannot be empty')
  }
  return valid
}
