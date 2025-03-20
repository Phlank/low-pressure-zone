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
import type { CommunityRequest } from '@/api/resources/communitiesApi.ts'
import type { PerformerRequest } from '@/api/resources/performersApi.ts'
import type { ScheduleRequest } from '@/api/resources/schedulesApi.ts'
import type { TimeslotRequest } from '@/api/resources/timeslotsApi.ts'
import type { LoginRequest, RegisterRequest } from '@/api/resources/authApi.ts'
import type { InviteRequest } from '@/api/resources/invitesApi.ts'

export const communityRequestRules: PropertyRules<CommunityRequest> = {
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
    communityId: required(),
    startsAt: combineRules(required(), hourOnly()),
    endsAt: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.startsAt, 60, 1440, '1 - 24h allowed')
    ),
    description: alwaysValid()
  }
}

export const timeslotRequestRules = (
  formState: TimeslotRequest
): PropertyRules<TimeslotRequest> => {
  return {
    performerId: required(),
    performanceType: required(),
    name: alwaysValid(),
    startsAt: combineRules(required(), hourOnly()),
    endsAt: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.startsAt, 60, 180, '1 - 3h allowed')
    )
  }
}

export const loginRequestRules: PropertyRules<LoginRequest> = {
  username: required(),
  password: required()
}

export const inviteRequestRules: PropertyRules<InviteRequest> = {
  email: combineRules(required(), emailAddress()),
  communityId: required()
}

export const registerRequestRules = (
  formState: RegisterRequest
): PropertyRules<RegisterRequest> => {
  return {
    context: alwaysValid(),
    username: combineRules(required(), allowedCharacters('A-Za-z0-9-._@+')),
    password: combineRules(
      minimumLength(8),
      requireAnyCharacter('A-Z', 'Requires uppercase'),
      requireAnyCharacter('a-z', 'Requires lowercase'),
      requireAnyCharacter('0-9', 'Requires number'),
      requireAnyOtherCharacter('A-Za-z0-9', 'Requires symbol')
    ),
    confirmPassword: equals(() => formState.password)
  }
}
