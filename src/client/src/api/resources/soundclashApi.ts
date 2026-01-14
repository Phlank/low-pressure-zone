import { sendDelete, sendGet, sendPost, sendPut } from '@/api/fetchFunctions.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'

const route = (id?: string) => `/soundclashes${id ? '/' + id : ''}`

export default {
  get: (scheduleId: string) => sendGet<SoundclashResponse[]>(route(), { scheduleId: scheduleId }),
  getById: (id: string) => sendGet<ApiResponse<SoundclashResponse>>(route(id)),
  post: (request: SoundclashRequest) => sendPost(route(), mapToRequest(request)),
  put: (id: string, request: SoundclashRequest) => sendPut(route(id), mapToRequest(request)),
  delete: (id: string) => sendDelete(route(id))
}

export interface SoundclashResponse {
  id: string
  scheduleId: string
  performerOne: PerformerResponse
  performerTwo: PerformerResponse
  roundOne: string
  roundTwo: string
  roundThree: string
  startsAt: string
  endsAt: string
  isEditable: boolean
  isDeletable: boolean
}

export interface SoundclashRequest {
  scheduleId: string
  performerOneId: string
  performerTwoId: string
  roundOne: string
  roundTwo: string
  roundThree: string
  startsAt: string
  endsAt: string
}

const mapToRequest = <TRequest extends SoundclashRequest>(request: TRequest) => ({
  scheduleId: request.scheduleId,
  performerOneId: request.performerOneId,
  performerTwoId: request.performerTwoId,
  roundOne: request.roundOne,
  roundTwo: request.roundTwo,
  roundThree: request.roundThree,
  startsAt: request.startsAt,
  endsAt: request.endsAt
})
