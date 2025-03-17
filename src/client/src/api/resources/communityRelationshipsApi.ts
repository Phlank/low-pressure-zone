import { sendGet, sendPut } from '@/api/fetchFunctions.ts'

const route = (communityId: string, userId?: string) =>
  `/communities/${communityId}/relationships/${userId ?? ''}`

export default {
  get: (communityId: string) => sendGet<CommunityRelationshipResponse[]>(route(communityId)),
  getById: (communityId: string, userId: string) =>
    sendGet<CommunityRelationshipResponse>(route(communityId, userId)),
  put: (communityId: string, userId: string, request: CommunityRelationshipRequest) =>
    sendPut(route(communityId, userId), mapRequest(request))
}

export interface CommunityRelationshipRequest {
  isPerformer: boolean
  isOrganizer: boolean
}

const mapRequest = <TRequest extends CommunityRelationshipRequest>(request: TRequest) => {
  return {
    isPerformer: request.isPerformer,
    isOrganizer: request.isOrganizer
  }
}

export interface CommunityRelationshipResponse {
  communityId: string
  userId: string
  username: string
  isPerformer: boolean
  isOrganizer: boolean
}
