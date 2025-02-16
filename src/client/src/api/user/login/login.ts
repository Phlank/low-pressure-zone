import { sendPost } from '@/api/fetchFunctions'
import type { LoginRequest } from './loginRequest'

const route = `/login`

export default {
  post: (request: LoginRequest) => sendPost(route, request)
}
