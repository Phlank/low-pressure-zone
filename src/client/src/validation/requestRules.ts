/* eslint-disable @typescript-eslint/no-unused-vars */
import type { AudienceRequest } from '@/api/audiences/audienceRequest'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import type { ScheduleRequest } from '@/api/schedules/scheduleRequest'
import type { TimeslotRequest } from '@/api/schedules/timeslots/timeslotRequest'
import type { InviteRequest } from '@/api/users/inviteRequest'
import type { LoginRequest } from '@/api/users/loginRequest'
import type { RegisterRequest } from '@/api/users/registerRequest'
import type { TwoFactorRequest } from '@/api/users/twoFactorRequest'
import { hourOnly, withinRangeOf } from './rules/dateStringRules'
import {
  allowedCharacters,
  emailAddress,
  equals,
  minimumLength,
  requireAnyCharacter,
  requireAnyOtherCharacter,
  required,
  url
} from './rules/stringRules'
import { alwaysValid } from './rules/untypedRules'
import type { PropertyRules } from './types/propertyRules'
import { combineRules } from './types/validationRule'

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

export const loginRequestRules: PropertyRules<LoginRequest> = {
  username: required(),
  password: required()
}

export const twoFactorRequestRules: PropertyRules<TwoFactorRequest> = {
  code: required()
}

export const inviteRequestRules: PropertyRules<InviteRequest> = {
  email: combineRules(required(), emailAddress())
}

export const registerRequestRules = (
  formState: RegisterRequest
): PropertyRules<RegisterRequest> => {
  return {
    context: alwaysValid(),
    username: combineRules(
      required(),
      allowedCharacters('ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._@+')
    ),
    password: combineRules(
      minimumLength(8),
      requireAnyCharacter('ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'Requires uppercase'),
      requireAnyCharacter('abcdefghijklmnopqrstuvwxyz', 'Requires lowercase'),
      requireAnyCharacter('0123456789', 'Requires number'),
      requireAnyOtherCharacter(
        'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789',
        'Requires symbol'
      )
    ),
    confirmPassword: equals(() => formState.password)
  }
}
