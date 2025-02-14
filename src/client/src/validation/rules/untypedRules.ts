/* eslint-disable @typescript-eslint/no-unused-vars */
import { valid } from '@/validation/types/validationResult'
import type { ValidationRule } from '@/validation/types/validationRule'

export const alwaysValid = () => () => valid
