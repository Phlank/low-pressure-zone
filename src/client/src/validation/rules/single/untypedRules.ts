import { valid } from '@/validation/types/validationResult'
import type { ValidationRule } from '@/validation/types/validationRule'

export const alwaysValid: ValidationRule<unknown> = () => valid
