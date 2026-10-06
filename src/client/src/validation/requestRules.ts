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
import { alwaysValid, applyRuleIf, equalTo, oneOf, required } from './rules/untypedRules'
import type { PropertyRules } from './types/propertyRules'
import { combineRules } from './types/validationRule'
import type { CommunityRequest } from '@/api/resources/communitiesApi.ts'
import type { PerformerRequest } from '@/api/resources/performersApi.ts'
import type { ScheduleRequest } from '@/api/resources/schedulesApi.ts'
import type { HourlySlotRequest } from '@/api/resources/hourlySlotsApi.ts'
import type { LoginRequest, RegisterRequest } from '@/api/resources/authApi.ts'
import type { InviteRequest } from '@/api/resources/invitesApi.ts'
import type { StreamerRequest } from '@/api/resources/usersApi.ts'
import { maxSize, mimeType } from '@/validation/rules/fileRules.ts'
import { audioMimeTypes } from '@/constants/audioMimeTypes.ts'
import type { Ref } from 'vue'
import type { ClashSlotRequest } from '@/api/resources/clashSlotsApi.ts'
import { arrayLength, each, notEmptyArray } from '@/validation/rules/arrayRules.ts'

export const communityRequestRules: PropertyRules<CommunityRequest> = {
  name: combineRules(required(), maximumLength(64)),
  socialUrl: combineRules(required(), url(), maximumLength(256))
}

export const performerRequestRules: PropertyRules<PerformerRequest> = {
  name: combineRules(required(), maximumLength(64)),
  socialUrl: combineRules(url(), maximumLength(256))
}

export const scheduleRequestRules = (
  formState: Ref<ScheduleRequest>
): PropertyRules<ScheduleRequest> => {
  return {
    name: combineRules(required(), maximumLength(64)),
    description: alwaysValid(),
    communityId: required(),
    startsAt: combineRules(required(), hourOnly()),
    endsAt: combineRules(
      required(),
      hourOnly(),
      withinRangeOf(() => formState.value.startsAt, 60, 1440, '1 - 24h allowed')
    ),
    isVisibleToPublic: alwaysValid(),
    isHourlyAllowed: applyRuleIf(
      equalTo(() => true, 'At least one slot type must be enabled'),
      () => !formState.value.isClashAllowed
    ),
    isClashAllowed: alwaysValid()
  }
}

export const hourlySlotRequestRules = (
  formState: Ref<HourlySlotRequest>
): PropertyRules<HourlySlotRequest> => {
  return {
    performerId: required(),
    startsAt: combineRules(required(), hourOnly()),
    duration: combineRules(required(), oneOf([1, 2, 3], 'Allowed values are 1, 2, or 3')),
    subtitle: maximumLength(64),
    file: combineRules<File | null>(
      mimeType(audioMimeTypes, 'Allowed audio types: wav, flac, mp3, m4a, vorbis, and opus.'),
      maxSize(1024 * 1024 * 1024, 'Max file size is 1GB.'),
      applyRuleIf(required(), () => formState.value.replaceMedia)
    ),
    replaceMedia: applyRuleIf(
      oneOf([false], 'Cannot replace and delete media simultaneously'),
      () => formState.value.deleteMedia
    ),
    deleteMedia: applyRuleIf(
      oneOf([false], 'Cannot replace and delete media simultaneously'),
      () => formState.value.replaceMedia
    )
  }
}

export const clashSlotRequestRules: PropertyRules<ClashSlotRequest> = {
  rounds: combineRules<string[]>(
    required(),
    notEmptyArray(),
    arrayLength(3, 3, 'Must have three rounds'),
    each(required())
  ),
  performerOneId: required(),
  performerTwoId: required(),
  startsAt: combineRules(required(), hourOnly()),
  duration: combineRules(
    required(),
    equalTo(() => 2)
  )
}

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
