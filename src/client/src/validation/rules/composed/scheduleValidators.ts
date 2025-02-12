import { combineRules } from '@/validation/types/validationRule'
import { required } from '../single/stringRules'
import { hourOnly, withinRangeOf } from '../single/dateStringRules'
import type { Ref } from 'vue'

export const audienceValidator = required()
export const startValidator = combineRules(required(), hourOnly())
export const endValidator = (start: Ref<string>) =>
  combineRules(required(), hourOnly(), withinRangeOf(start, 60, 1440, 'Out of range (1-24h)'))
