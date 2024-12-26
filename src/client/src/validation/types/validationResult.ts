export interface ValidationResult {
  isValid: boolean
  message: string | null
}

export const valid: ValidationResult = {
  isValid: true,
  message: null
}

export const invalid = (message: string): ValidationResult => {
  return {
    isValid: false,
    message: message
  }
}
