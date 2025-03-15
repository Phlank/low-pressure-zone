import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'

const route = (performerId?: string) => `/performers${performerId ? '/' + performerId : ''}`

export default {
  get: () => sendGet<PerformerResponse[]>(route()),
  getById: (id: string) => sendGet<PerformerResponse>(route(id)),
  post: (request: PerformerRequest) => sendPost(route(), request),
  put: (id: string, request: PerformerRequest) => sendPut(route(id), request),
  delete: (id: string) => sendDelete(route(id))
}

export interface PerformerRequest {
  name: string
  url: string
}

export interface PerformerResponse {
  id: string
  name: string
  url: string
  isDeletable: boolean
  isEditable: boolean
  isLinkableToTimeslot: boolean
}
