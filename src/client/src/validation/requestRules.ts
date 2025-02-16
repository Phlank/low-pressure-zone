/* eslint-disable @typescript-eslint/no-unused-vars */
import type { AudienceRequest } from '@/api/audiences/audienceRequest'
import { required, url } from './rules/stringRules'
import { combineRules } from './types/validationRule'
import type { PropertyRules } from './types/propertyRules'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import type { ScheduleRequest } from '@/api/schedules/scheduleRequest'
import type { TimeslotRequest } from '@/api/schedules/timeslots/timeslotRequest'
import { hourOnly, withinRangeOf } from './rules/dateStringRules'
import type { RequestValidator } from './types/requestValidator'
import { alwaysValid } from './rules/untypedRules'

export const audienceRequestRules: PropertyRules<AudienceRequest> = {
  name: required(),
  url: combineRules(required(), url())
}

export const performerRequestRules: PropertyRules<PerformerRequest> = {
  name: required(),
  url: combineRules(required(), url())
}

export const scheduleRequestRules = (
  formState: ScheduleRequest
): PropertyRules<ScheduleRequest> => {
  return {
    audienceId: required(),
    start: combineRules(required(), hourOnly()),
    end: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.start, 60, 1440, '1 - 24h allowed')
    )
  }
}

export const timeslotRequestRules = (
  formState: TimeslotRequest
): PropertyRules<TimeslotRequest> => {
  return {
    performerId: required(),
    performanceType: required(),
    name: alwaysValid(),
    start: combineRules(required(), hourOnly()),
    end: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.start, 60, 180, '1 - 3h allowed')
    )
  }
}

export const loginRequestRules = {
  email: required(),
  password: required()
}
