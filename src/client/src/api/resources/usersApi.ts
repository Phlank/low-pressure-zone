import { sendGet } from '../fetchFunctions.ts'

const route = '/users'

export default {
  get: () => sendGet<UserResponse[]>(route)
}

export interface UserResponse {
  id: string
  username: string
  email: string
  registrationDate: string
  roles: string[]
}
