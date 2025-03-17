import { sendGet } from '../fetchFunctions.ts'

const route = '/users'

export default {
  get: () => sendGet<UserResponse[]>(route),
  getUsernames: () => sendGet<UsernameResponse[]>(`${route}/usernames`)
}

export interface UserResponse {
  id: string
  username: string
  email: string
  registrationDate: string
  roles: string[]
}

export interface UsernameResponse {
  id: string
  username: string
}
