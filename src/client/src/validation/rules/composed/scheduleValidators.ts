import { combineRules } from '@/validation/types/validationRule'
import { required } from '../single/stringRules'
import { withinRangeOf } from '../single/dateStringRules'
import type { Ref } from 'vue'

export const audienceValidator = required()
export const startValidator = required()
export const endValidator = (start: Ref<string>) =>
  combineRules(required(), withinRangeOf(start, 60, 1440, 'Out of range (1-24h)'))
