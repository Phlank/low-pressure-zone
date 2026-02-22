import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import type { TimeslotResponse } from '@/api/resources/timeslotsApi.ts'
import type {SoundclashResponse} from "@/api/resources/soundclashApi.ts";

const route = (scheduleId?: string) => `/schedules${scheduleId ? '/' + scheduleId : ''}`

export default {
  get: (params?: { before?: string; after?: string }) =>
    sendGet<ScheduleResponse[]>(route(), params),
  getById: (id: string) => sendGet<ScheduleResponse>(route(id)),
  post: <TSchedule extends ScheduleRequest>(request: TSchedule) =>
    sendPost<ScheduleRequest>(route(), mapRequest(request)),
  put: <TSchedule extends ScheduleRequest>(id: string, request: TSchedule) =>
    sendPut<ScheduleRequest>(route(id), mapRequest(request)),
  delete: (id: string) => sendDelete(route(id)),
  mapResponseToRequest: (response: ScheduleResponse) => mapResponseToRequest(response)
}

export interface ScheduleRequest {
  type: string
  startsAt: string
  endsAt: string
  name: string
  description: string
  communityId: string
  isOrganizersOnly: boolean
}

export interface ScheduleResponse {
  id: string
  type: string
  startsAt: string
  endsAt: string
  name: string
  description: string
  community: CommunityResponse
  timeslots: TimeslotResponse[]
  soundclashes: SoundclashResponse[]
  isEditable: boolean
  isDeletable: boolean
  isTimeslotCreationAllowed: boolean
  isSoundclashCreationAllowed: boolean
  isOrganizersOnly: boolean
}

const mapRequest = <TSchedule extends ScheduleRequest>(schedule: TSchedule): ScheduleRequest => {
  return {
    type: schedule.type,
    communityId: schedule.communityId,
    startsAt: schedule.startsAt,
    endsAt: schedule.endsAt,
    name: schedule.name,
    description: schedule.description,
    isOrganizersOnly: schedule.isOrganizersOnly
  }
}

const mapResponseToRequest = (response: ScheduleResponse): ScheduleRequest => {
  return {
    type: response.type,
    communityId: response.community.id,
    startsAt: response.startsAt,
    endsAt: response.endsAt,
    name: response.name,
    description: response.description,
    isOrganizersOnly: response.isOrganizersOnly
  }
}
