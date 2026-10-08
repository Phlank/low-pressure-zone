import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import type { HourlySlotResponse } from '@/api/resources/hourlySlotsApi.ts'
import type { ClashSlotResponse } from '@/api/resources/clashSlotsApi.ts'

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
  name: string
  description: string
  communityId: string
  startsAt: string
  endsAt: string
  isHourlyAllowed: boolean
  isClashAllowed: boolean
  isVisibleToPublic: boolean
}

export interface ScheduleResponse {
  id: string
  startsAt: string
  endsAt: string
  name: string
  description: string
  community: CommunityResponse
  slots: SlotResponse[]
  isEditable: boolean
  isDeletable: boolean
  isHourlyAllowed: boolean
  isClashAllowed: boolean
  isVisibleToPublic: boolean
  isHourlySlotCreationAllowed: boolean
  isClashSlotCreationAllowed: boolean
}

export type SlotResponse = HourlySlotResponse | ClashSlotResponse

const mapRequest = <TSchedule extends ScheduleRequest>(schedule: TSchedule): ScheduleRequest => {
  return {
    name: schedule.name,
    description: schedule.description,
    communityId: schedule.communityId,
    startsAt: schedule.startsAt,
    endsAt: schedule.endsAt,
    isHourlyAllowed: schedule.isHourlyAllowed,
    isClashAllowed: schedule.isClashAllowed,
    isVisibleToPublic: schedule.isVisibleToPublic
  }
}

const mapResponseToRequest = (response: ScheduleResponse): ScheduleRequest => {
  return {
    communityId: response.community.id,
    startsAt: response.startsAt,
    endsAt: response.endsAt,
    name: response.name,
    description: response.description,
    isHourlyAllowed: response.isHourlyAllowed,
    isClashAllowed: response.isClashAllowed,
    isVisibleToPublic: response.isVisibleToPublic
  }
}
