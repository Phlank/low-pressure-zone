import communitiesApi, {
  type CommunityRequest,
  type CommunityResponse
} from '@/api/resources/communitiesApi'
import { defineStore } from 'pinia'
import { computed, type Ref, ref } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipRequest,
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import { useToast } from 'primevue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import { type Role, roles } from '@/constants/roles.ts'
import { err, ok, type Result } from '@/types/result.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'
import { getEntity, removeEntity } from '@/utils/arrayUtils.ts'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useUserStore } from '@/stores/userStore.ts'
import { useRefresh } from '@/composables/useRefresh.ts'

export const useCommunityStore = defineStore('communityStore', () => {
  const communities: Ref<CommunityResponse[]> = ref([])
  const relationships: Ref<Record<string, CommunityRelationshipResponse[]>> = ref({})
  const toast = useToast()
  const auth = useAuthStore()
  const users = useUserStore()

  const { isLoading } = useRefresh(
    communitiesApi.get,
    (data) => {
      communities.value = data
      refreshRelationships().then(() => {})
    },
    { permissionFn: () => auth.isLoggedIn }
  )

  const refreshRelationships = async () => {
    if (!auth.isInAnyRoles(roles.admin, roles.organizer)) return
    const userCommunities = communities.value.filter((community) => community.isOrganizable)
    const relationshipPromises: Promise<ApiResponse<void, CommunityRelationshipResponse[]>>[] = []
    for (const community of userCommunities) {
      relationshipPromises.push(communityRelationshipsApi.get(community.id))
    }
    await Promise.all(relationshipPromises)
    for (const relationshipPromise of relationshipPromises) {
      const response = await relationshipPromise
      if (tryHandleUnsuccessfulResponse(response, toast)) continue
      if (response.data().length === 0) continue
      relationships.value[response.data()[0]!.communityId] = response.data()
    }
  }

  const getCommunity = (id: string) => communities.value.find((community) => community.id === id)
  const getCommunities = computed(() => communities.value)
  const getRelatedCommunities = computed(() =>
    communities.value.filter((community) => community.isPerformable || community.isOrganizable)
  )
  const getOrganizableCommunities = computed(() =>
    communities.value.filter((community) => community.isOrganizable)
  )
  const getPerformableCommunities = computed(() =>
    communities.value.filter((community) => community.isPerformable)
  )

  const createCommunity = async (
    formState: Ref<CommunityRequest>,
    validation: FormValidation<CommunityRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await communitiesApi.post(formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    const entity: CommunityResponse = {
      id: response.getCreatedId(),
      ...formState.value,
      isEditable: true,
      isOrganizable: true,
      isPerformable: true,
      isDeletable: true
    }
    communities.value.unshift(entity)
    showSuccessToast(toast, 'Created', 'Community', formState.value.name)
    return ok()
  }

  const updateCommunity = async (
    id: string,
    formState: Ref<CommunityRequest>,
    validation: FormValidation<CommunityRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await communitiesApi.put(id, formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    const entity = getEntity(communities.value, id)
    if (!entity) return err()
    entity.name = formState.value.name
    entity.url = formState.value.url
    return ok()
  }

  const removeCommunity = async (id: string): Promise<Result> => {
    const response = await communitiesApi.delete(id)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    const name = getEntity(communities.value, id)?.name
    showSuccessToast(toast, 'Deleted', 'Community', name)
    removeEntity(communities.value, id)
    relationships.value[id] = []
    return ok()
  }

  const getRelationships = (communityId: string) => relationships.value[communityId] ?? []
  const getRelationship = (communityId: string, userId: string) =>
    relationships.value[communityId]?.find((relationship) => relationship.userId === userId)

  const createRelationship = async (
    communityId: string,
    userId: string,
    formState: Ref<CommunityRelationshipRequest>,
    validation: FormValidation<CommunityRelationshipRequest>
  ): Promise<Result> => {
    const existingEntity = getRelationship(communityId, userId)
    if (existingEntity) return err()
    if (!validation.validate()) return err()
    const response = await communityRelationshipsApi.put(communityId, userId, formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    const user = users.getUser(userId)
    const entity: CommunityRelationshipResponse = {
      userId: userId,
      communityId: communityId,
      ...formState.value,
      isEditable: true,
      displayName: user?.displayName ?? ''
    }
    relationships.value[communityId] ??= []
    relationships.value[communityId].unshift(entity)
    showSuccessToast(toast, 'Created', 'Community Relationship', user?.displayName)
    return ok()
  }
  const updateRelationship = async (
    communityId: string,
    userId: string,
    formState: Ref<CommunityRelationshipRequest>,
    validation: FormValidation<CommunityRelationshipRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await communityRelationshipsApi.put(communityId, userId, formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    const entity = getRelationship(communityId, userId)
    if (!entity) return err()
    entity.isOrganizer = formState.value.isOrganizer
    entity.isPerformer = formState.value.isPerformer
    showSuccessToast(toast, 'Updated', 'Community Relationship')
    return ok()
  }
  const removeRelationship = async (communityId: string, userId: string): Promise<Result> => {
    const entity = getRelationship(communityId, userId)
    if (!entity) return err()
    const response = await communityRelationshipsApi.put(communityId, userId, {
      isOrganizer: false,
      isPerformer: false
    })
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    showSuccessToast(toast, 'Deleted', 'Community Relationship', entity.displayName)
    const index = relationships.value[communityId]!.findIndex(
      (relationship) => relationship.userId === userId
    )
    if (index === -1) return err()
    relationships.value[communityId]?.splice(index, 1)
    showSuccessToast(toast, 'Deleted', 'Community Relationship', entity.displayName)
    return ok()
  }

  const getCommunityRoles = (communityId: string): Role[] => {
    const userRelationship = getRelationships(communityId).find(
      (rel) => rel.userId === auth.user.id
    )
    if (userRelationship === undefined) return []
    const userRoles: Role[] = []
    if (userRelationship.isOrganizer) userRoles.push(roles.organizer as Role)
    if (userRelationship.isPerformer) userRoles.push(roles.performer as Role)
    return userRoles
  }

  const getIsLoading = computed(() => isLoading.value)

  return {
    communities: getCommunities,
    getCommunityById: getCommunity,
    relatedCommunities: getRelatedCommunities,
    organizableCommunities: getOrganizableCommunities,
    performableCommunities: getPerformableCommunities,
    createCommunity,
    updateCommunity,
    removeCommunity,
    getRelationships,
    getRelationship,
    createRelationship,
    updateRelationship,
    removeRelationship,
    getCommunityRoles,
    isLoading: getIsLoading
  }
})
