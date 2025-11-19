import { invalid, valid, type ValidationResult } from '@/validation/types/validationResult.ts'

export const mimeType =
  (allowedTypes: string[], msg?: string) =>
  (file: File | null): ValidationResult => {
    if (!file) return valid
    if (allowedTypes.includes(file.type)) return valid
    return invalid(msg ?? `Invalid file type. Allowed types: ${allowedTypes.join(', ')}`)
  }

  export const maxSize = (maxBytes: number, msg?: string) => (file: File | null): ValidationResult => {
    if (!file) return valid
    if (file.size <= maxBytes) return valid
    return invalid(msg ?? `Max ${maxBytes} bytes`)
  }
