import { invalid, valid } from '@/validation/types/validationResult.ts'

export const notEmptyArray = (msg?: string) => <T>(value?: T[]) => {
  if (!value) return valid
  if (value.length === 0) {
    return invalid(msg ?? 'Cannot be empty')
  }
  return valid
}
