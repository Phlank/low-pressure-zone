import { sendGet, sendPost } from '../fetchFunctions.ts'

const route = '/users'

export default {
  get: () => sendGet<UserResponse[]>(route),
  createStreamer: (userId: string) => sendPost(`${route}/streamers/link/${userId}`),
  createStreamers: () => sendPost(`${route}/streamers/link`)
}

export interface UserResponse {
  id: string
  displayName: string
  registrationDate: string
  isAdmin: boolean
  isStreamer: boolean
}
