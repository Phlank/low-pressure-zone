import { sendDelete, sendGet, sendPost, sendPut } from '../fetchFunctions'

const route = (performerId?: string) => `/performers/${performerId ?? ''}`

export default {
  get: () => sendGet<PerformerResponse[]>(route()),
  getById: (id: string) => sendGet<PerformerResponse>(route(id)),
  post: (request: PerformerRequest) => sendPost(route(), mapRequest(request)),
  put: (id: string, request: PerformerRequest) => sendPut(route(id), mapRequest(request)),
  delete: (id: string) => sendDelete(route(id))
}

export interface PerformerRequest {
  name: string
  url: string
}

const mapRequest = <TRequest extends PerformerRequest>(request: TRequest) => {
  return {
    name: request.name,
    url: request.url
  }
}

export interface PerformerResponse {
  id: string
  name: string
  url: string
  isDeletable: boolean
  isEditable: boolean
  isLinkableToTimeslot: boolean
}
