import { sendGet, sendPost } from '../fetchFunctions.ts'

const route = '/users'

export default {
  get: () => sendGet<UserResponse[]>(route),
  createStreamers: () => sendPost(`${route}/streamers`)
}

export interface UserResponse {
  id: string
  displayName: string
  registrationDate: string
  isAdmin: boolean
  isStreamer: boolean
}
