import { sendDelete, sendGet, sendPost, sendPut } from '@/api/fetchFunctions.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'

const route = (id?: string) => `/soundclash${id ? '/' + id : ''}`

export default {
  getById: (id: string) => sendGet<ApiResponse<SoundclashResponse>>(route(id)),
  post: (request: SoundclashRequest) => sendPost(route(), request),
  put: (id: string, request: SoundclashRequest) => sendPut(route(id), request),
  delete: (id: string) => sendDelete(route(id)),
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
  scheduleId: string,
  performerOneId: string,
  performerTwoId: string,
  roundOne: string,
  roundTwo: string,
  roundThree: string,
  startsAt: string,
  endsAt: string
}
