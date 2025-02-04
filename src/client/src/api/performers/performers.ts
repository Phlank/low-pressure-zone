import { sendGet, sendPost, sendPut } from '../sendRequest'
import { type PerformerRequest } from './performerRequest'
import { type PerformerResponse } from './performerResponse'

const route = (performerId?: string) => `/api/performers${performerId ? '/' + performerId : ''}`

export default {
  get: () => sendGet<PerformerResponse[]>(route()),
  getById: (id: string) => sendGet<PerformerResponse>(route(id)),
  post: (request: PerformerRequest) => sendPost(route(), request),
  put: (id: string, request: PerformerRequest) => sendPut(route(id), request)
}
