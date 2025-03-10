import { sendGet, sendPost } from '@/api/fetchFunctions'
import type { InviteRequest } from './inviteRequest'
import type { InviteResponse } from './inviteResponse'

const route = '/users/invites'

export default {
  get: () => sendGet<InviteResponse[]>(route),
  post: (request: InviteRequest) => sendPost(route, request),
  resend: (email: string) => sendGet<never>(`${route}/resend`, { email: email })
}
