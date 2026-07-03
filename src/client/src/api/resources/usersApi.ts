import { sendGet, sendPost, sendPut } from '../fetchFunctions.ts'

const route = (id?: string) => id ? `/users/${id}` : '/users'

export default {
  get: () => sendGet<UserResponse[]>(route()),
  createStreamer: (userId: string) => sendPost(`${route()}/streamers/link/${userId}`),
  createStreamers: () => sendPost(`${route()}/streamers/link`),
  putStreamer: (request: StreamerRequest) =>
    sendPut<StreamerRequest>(`${route()}/streamers`, request),
  getStreamerPassword: () => sendGet<StreamerPasswordResponse>(`${route()}/streamers/password`),
  putEnabled: (id: string, enabled: boolean) => sendPut(`${route(id)}/enabled`, { enabled: enabled })
}

export interface UserResponse {
  id: string
  displayName: string
  registrationDate: string
  isAdmin: boolean
  isStreamer: boolean
  canBeEnabled: boolean
  canBeDisabled: boolean
}

export interface StreamerRequest {
  displayName: string
}

export interface StreamerPasswordResponse {
  password: string
}
