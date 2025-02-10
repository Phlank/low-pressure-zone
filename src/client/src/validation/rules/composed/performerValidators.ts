import { combineRules } from '@/validation/types/validationRule'
import { required, url } from '../single/stringRules'

export const nameValidator = required()
export const urlValidator = combineRules(required(), url())
