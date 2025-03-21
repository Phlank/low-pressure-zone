import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi'
import { defineStore } from 'pinia'
import { type Ref, ref } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'

export const useCommunityStore = defineStore('communityStore', () => {
  const loadedCommunities: Ref<CommunityResponse[]> = ref([])
  const loadedCommunityRelationships: Ref<RelationshipMap> = ref({})

  let loadCommunitiesPromise: Promise<void> | undefined = undefined
  const loadCommunities = async () => {
    const response = await communitiesApi.get()
    if (!response.isSuccess()) {
      console.log(JSON.stringify(response.error))
      return
    }
    loadedCommunities.value = response.data!
  }

  const loadCommunitiesAsync = async () => {
    if (!loadCommunitiesPromise) {
      loadCommunitiesPromise = loadCommunities()
    }
    await loadCommunitiesPromise
    loadCommunitiesPromise = undefined
  }
  const getCommunities = () => loadedCommunities.value

  const loadRelationshipsPromises: RelationshipPromiseMap = {}
  const loadRelationships = async (communityId: string) => {
    const response = await communityRelationshipsApi.get(communityId)
    if (!response.isSuccess()) {
      console.log(JSON.stringify(response.error))
      return
    }
    loadedCommunityRelationships.value[communityId] = response.data!
  }

  const loadRelationshipsAsync = async (communityId: string) => {
    if (!loadRelationshipsPromises[communityId]) {
      loadRelationshipsPromises[communityId] = loadRelationships(communityId)
    }
    await loadRelationshipsPromises[communityId]
    loadRelationshipsPromises[communityId] = undefined
  }
  const getRelationships = (communityId: string) =>
    loadedCommunityRelationships.value[communityId] ?? []

  return {
    loadCommunitiesAsync,
    getCommunities,
    loadRelationshipsAsync,
    getRelationships
  }
})

type RelationshipMap = Record<string, CommunityRelationshipResponse[] | undefined>
type RelationshipPromiseMap = Record<string, Promise<void> | undefined>
