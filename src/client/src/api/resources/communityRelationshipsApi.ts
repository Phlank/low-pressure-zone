import { sendGet, sendPost } from '@/api/fetchFunctions.ts'

const route = (communityId: string, userId?: string) =>
  `/communities/${communityId}/relationships/${userId ?? ''}`

export default {
  get: (communityId: string) => sendGet<CommunityRelationshipResponse[]>(route(communityId)),
  getById: (communityId: string, userId: string) =>
    sendGet<CommunityRelationshipResponse>(route(communityId, userId)),
  post: (communityId: string, userId: string, request: CommunityRelationshipRequest) =>
    sendPost(route(communityId, userId), request)
}

export interface CommunityRelationshipRequest {
  isPerformer: boolean
  isOrganizer: boolean
}

export interface CommunityRelationshipResponse {
  communityId: string
  userId: string
  username: string
  isPerformer: boolean
  isOrganizer: boolean
}
