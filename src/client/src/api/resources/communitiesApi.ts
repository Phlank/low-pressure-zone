import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'

const route = (id?: string) => `/communities/${id ?? ''}`

export default {
  get: () => sendGet<CommunityResponse[]>(route()),
  getById: (id: string) => sendGet<CommunityResponse>(route(id)),
  post: (request: CommunityRequest) => sendPost(route(), request),
  put: (id: string, request: CommunityRequest) => sendPut(route(id), request),
  delete: (id: string) => sendDelete(route(id))
}

export interface CommunityRequest {
  name: string
  url: string
}

export interface CommunityResponse {
  id: string
  name: string
  url: string
  isPerformable: boolean
  isEditable: boolean
  isDeletable: boolean
  isOrganizable: boolean
}
