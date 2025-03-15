import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import type { TimeslotResponse } from '@/api/resources/timeslotsApi.ts'

const route = (scheduleId?: string) => `/schedules${scheduleId ? '/' + scheduleId : ''}`

export default {
  get: (params?: { before?: string; after?: string }) =>
    sendGet<ScheduleResponse[]>(route(), params),
  getById: (id: string) => sendGet<ScheduleResponse>(route(id)),
  post: <TSchedule extends ScheduleRequest>(request: TSchedule) =>
    sendPost<ScheduleRequest>(route(), mapRequest(request)),
  put: <TSchedule extends ScheduleRequest>(id: string, request: TSchedule) =>
    sendPut<ScheduleRequest>(route(id), mapRequest(request)),
  delete: (id: string) => sendDelete(route(id))
}

export interface ScheduleRequest {
  startsAt: string
  endsAt: string
  description: string
  communityId: string
}

export interface ScheduleResponse {
  id: string
  startsAt: string
  endsAt: string
  description: string
  community: CommunityResponse
  timeslots: TimeslotResponse[]
  isEditable: boolean
  isDeletable: boolean
  isTimeslotCreationAllowed: boolean
}

// Forms will deal with dates, but we don't want to send the actual dates into the API
const mapRequest = <TSchedule extends ScheduleRequest>(schedule: TSchedule): ScheduleRequest => {
  return {
    communityId: schedule.communityId,
    startsAt: schedule.startsAt,
    endsAt: schedule.endsAt,
    description: schedule.description
  }
}
