import { sendGet, sendPost } from '@/api/fetchFunctions.ts'
import type { Role } from '@/constants/roles.ts'

const route = '/users/invites'

export default {
  get: () => sendGet<InviteResponse[]>(route),
  post: (request: InviteRequest) => sendPost(route, request),
  getResend: (email: string) => sendGet<never>(`${route}/resend`, { email: email })
}

export interface InviteRequest {
  email: string
  role: Role
}

export interface InviteResponse {
  id: string
  email: string
  invitedAt: string
  lastSentAt: string
}
