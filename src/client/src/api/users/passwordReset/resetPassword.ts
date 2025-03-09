import { sendGet, sendPost } from '@/api/fetchFunctions'
import type { ResetPasswordRequest } from './resetPasswordRequest'

const route = '/users/resetpassword'

export default {
  get: (email: string) => sendGet<never>(route, { email: email }),
  post: (request: ResetPasswordRequest) => sendPost(route, request)
}
