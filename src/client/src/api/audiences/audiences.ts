import { sendGet, sendPost, sendPut } from '../axiosInstance'
import type { AudienceRequest } from './audienceRequest'
import type { AudienceResponse } from './audienceResponse'

const route = (id?: string) => `/audiences${id ? '/' + id : ''}`

export default {
  get: () => sendGet<AudienceResponse[]>(route()),
  getById: (id: string) => sendGet<AudienceResponse>(route(id)),
  post: (request: AudienceRequest) => sendPost(route(), request),
  put: (id: string, request: AudienceRequest) => sendPut(route(id), request)
}
