import { hourOnly, withinRangeOf } from './rules/dateStringRules'
import {
  allowedCharacters,
  emailAddress,
  equals,
  maximumLength,
  minimumLength,
  requireAnyCharacter,
  requireAnyOtherCharacter,
  url
} from './rules/stringRules'
import { alwaysValid, applyRuleIf, oneOf, required } from './rules/untypedRules'
import type { PropertyRules } from './types/propertyRules'
import { combineRules } from './types/validationRule'
import type { CommunityRequest } from '@/api/resources/communitiesApi.ts'
import type { PerformerRequest } from '@/api/resources/performersApi.ts'
import type { ScheduleRequest } from '@/api/resources/schedulesApi.ts'
import type { TimeslotRequest } from '@/api/resources/timeslotsApi.ts'
import type { LoginRequest, RegisterRequest } from '@/api/resources/authApi.ts'
import type { InviteRequest } from '@/api/resources/invitesApi.ts'
import type { StreamerRequest } from '@/api/resources/usersApi.ts'
import { maxSize, mimeType } from '@/validation/rules/fileRules.ts'
import { audioMimeTypes } from '@/constants/audioMimeTypes.ts'
import { scheduleTypes } from '@/constants/scheduleTypes.ts'
import type { SoundclashRequest } from '@/api/resources/soundclashApi.ts'
import type { Ref } from 'vue'

export const communityRequestRules: PropertyRules<CommunityRequest> = {
  name: combineRules(required(), maximumLength(64)),
  url: combineRules(required(), url(), maximumLength(256))
}

export const performerRequestRules: PropertyRules<PerformerRequest> = {
  name: combineRules(required(), maximumLength(64)),
  url: combineRules(url(), maximumLength(256))
}

export const scheduleRequestRules = (
  formState: Ref<ScheduleRequest>
): PropertyRules<ScheduleRequest> => {
  return {
    type: combineRules(
      required(),
      oneOf([scheduleTypes.Hourly, scheduleTypes.Soundclash], 'Invalid schedule type')
    ),
    communityId: required(),
    startsAt: combineRules(required(), hourOnly()),
    endsAt: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.value.startsAt, 60, 1440, '1 - 24h allowed')
    ),
    name: combineRules(required(), maximumLength(64)),
    description: alwaysValid(),
    isOrganizersOnly: alwaysValid()
  }
}

export const timeslotRequestRules = (
  formState: Ref<TimeslotRequest>
): PropertyRules<TimeslotRequest> => {
  return {
    scheduleId: required(),
    performerId: required(),
    performanceType: required(),
    startsAt: combineRules(required(), hourOnly()),
    endsAt: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.value.startsAt, 60, 180, '1 - 3h allowed')
    ),
    subtitle: maximumLength(64),
    file: combineRules<File | null>(
      mimeType(audioMimeTypes, 'Allowed audio types: wav, flac, mp3, m4a, vorbis, and opus.'),
      maxSize(1024 * 1024 * 1024, 'Max file size is 1GB.'),
      applyRuleIf(required(), () => formState.value.performanceType === 'Prerecorded DJ Set')
    ),
    replaceMedia: alwaysValid()
  }
}

export const soundclashRequestRules = (
  formState: Ref<SoundclashRequest>
): PropertyRules<SoundclashRequest> => ({
  roundOne: combineRules(required()),
  roundTwo: combineRules(required()),
  roundThree: combineRules(required()),
  performerOneId: required(),
  performerTwoId: required(),
  scheduleId: required(),
  startsAt: combineRules(required(), hourOnly()),
  endsAt: combineRules(
    required(),
    hourOnly(),
    withinRangeOf(() => formState.value.startsAt, 120, 120, '2h allowed')
  )
})

export const loginRequestRules: PropertyRules<LoginRequest> = {
  username: required(),
  password: required()
}

export const inviteRequestRules: PropertyRules<InviteRequest> = {
  email: combineRules(required(), emailAddress()),
  communityId: required(),
  isPerformer: alwaysValid(),
  isOrganizer: alwaysValid()
}

export const registerRequestRules = (
  formState: RegisterRequest
): PropertyRules<RegisterRequest> => {
  return {
    context: alwaysValid(),
    displayName: combineRules(required(), maximumLength(64)),
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

export const streamerRequestRules: PropertyRules<StreamerRequest> = {
  displayName: combineRules(required(), maximumLength(128))
}
