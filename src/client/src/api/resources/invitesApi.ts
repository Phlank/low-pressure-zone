import { sendGet, sendPost } from '@/api/fetchFunctions.ts'

const route = '/users/invites'

export default {
  get: () => sendGet<InviteResponse[]>(route),
  post: (request: InviteRequest) => sendPost(route, request),
  getResend: (email: string) => sendGet<never>(`${route}/resend`, { email: email })
}

export interface InviteRequest {
  email: string
  communityId: string
  isPerformer: boolean
  isOrganizer: boolean
}

export interface InviteResponse {
  id: string
  communityId: string
  email: string
  displayName: string
  invitedAt: string
  lastSentAt: string
}
