import { sendPost } from '@/api/fetchFunctions'
import type { TwoFactorRequest } from './twoFactorRequest'

const route = `/users/twofactor`

export default {
  post: (request: TwoFactorRequest) => sendPost(route, request)
}
