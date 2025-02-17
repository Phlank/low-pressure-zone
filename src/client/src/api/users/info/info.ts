import { sendGet } from '@/api/fetchFunctions'
import type { UserResponse } from '../userResponse'

export default {
  get: () => sendGet<UserResponse>(route)
}
