import { sendGet } from '@/api/fetchFunctions'
import type { UserResponse } from '../userResponse'

const route = '/users/info'

export default {
  get: () => sendGet<UserResponse>(route)
}
