export interface ValidationResult {
  isValid: boolean
  message: string
}

export const valid: ValidationResult = {
  isValid: true,
  message: ''
}

export const invalid = (message: string): ValidationResult => {
  return {
    isValid: false,
    message: message
  }
}
