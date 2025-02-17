import { sendPost } from '@/api/fetchFunctions'
import type { LoginRequest } from './loginRequest'
import type { LoginResponse } from './loginResponse'

const route = `/users/login`

export default {
  post: (request: LoginRequest) => sendPost<LoginRequest, LoginResponse>(route, request)
}
