import { sendGet, sendPost, sendPut } from '../fetchFunctions.ts'

const route = '/users'

export default {
  get: () => sendGet<UserResponse[]>(route),
  createStreamer: (userId: string) => sendPost(`${route}/streamers/link/${userId}`),
  createStreamers: () => sendPost(`${route}/streamers/link`),
  putStreamer: (request: StreamerRequest) =>
    sendPut<StreamerRequest>(`${route}/streamers`, request),
  getStreamerPassword: () => sendGet<StreamerPasswordResponse>(`${route}/streamers/password`)
}

export interface UserResponse {
  id: string
  displayName: string
  registrationDate: string
  isAdmin: boolean
  isStreamer: boolean
}

export interface StreamerRequest {
  displayName: string
}

export interface StreamerPasswordResponse {
  password: string
}
