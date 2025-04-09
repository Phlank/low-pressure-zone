import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi'
import { defineStore } from 'pinia'
import { computed, type Ref, ref } from 'vue'
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
    if (loadCommunitiesPromise === undefined) {
      loadCommunitiesPromise = loadCommunities()
    }
    await loadCommunitiesPromise
    loadCommunitiesPromise = undefined
  }

  const communities = computed(() => loadedCommunities.value)

  const relatedCommunities = computed(() =>
    communities.value.filter((community) => community.isPerformable || community.isOrganizable)
  )

  const organizableCommunities = computed(() =>
    communities.value.filter((community) => community.isOrganizable)
  )

  const performableCommunities = computed(() =>
    communities.value.filter((community) => community.isPerformable)
  )

  const removeCommunity = (id: string) => {
    const index = loadedCommunities.value.findIndex((community) => community.id === id)
    if (index > -1) {
      loadedCommunities.value.splice(index, 1)
    }
  }

  const getCommunity = (id: string) => communities.value.find((community) => community.id === id)

  const addCommunity = (community: CommunityResponse) => {
    const alphabeticalIndex = loadedCommunities.value.findIndex(
      (loadedCommunity) => loadedCommunity.name.toLowerCase() > community.name.toLowerCase()
    )
    if (alphabeticalIndex > -1) {
      loadedCommunities.value.splice(alphabeticalIndex, 0, community)
    } else {
      loadedCommunities.value.push(community)
    }
  }

  const updateCommunityAsync = async (id: string) => {
    const response = await communitiesApi.getById(id)
    if (!response.isSuccess()) return
    const index = loadedCommunities.value.findIndex((community) => community.id === id)
    if (index > -1) {
      loadedCommunities.value.splice(index, 1, response.data!)
    } else {
      addCommunity(response.data!)
    }
  }

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
    if (loadRelationshipsPromises[communityId] === undefined) {
      loadRelationshipsPromises[communityId] = loadRelationships(communityId)
    }
    await loadRelationshipsPromises[communityId]
    loadRelationshipsPromises[communityId] = undefined
  }
  const getRelationships = (communityId: string) =>
    loadedCommunityRelationships.value[communityId] ?? []

  const addRelationship = (communityId: string, relationship: CommunityRelationshipResponse) => {
    if (loadedCommunityRelationships.value[communityId] === undefined) return
    const alphabeticalIndex = loadedCommunityRelationships.value[communityId].findIndex(
      (loadedRelationship) =>
        loadedRelationship.displayName.toLowerCase() > relationship.displayName.toLowerCase()
    )
    if (alphabeticalIndex > -1) {
      loadedCommunityRelationships.value[communityId].splice(alphabeticalIndex, 0, relationship)
    } else {
      loadedCommunityRelationships.value[communityId].push(relationship)
    }
  }

  const removeRelationship = (communityId: string, userId: string) => {
    if (loadedCommunityRelationships.value[communityId] === undefined) return
    const index = loadedCommunityRelationships.value[communityId]?.findIndex(
      (relationship) => relationship.userId === userId
    )
    if (index === -1) return
    loadedCommunityRelationships.value[communityId].splice(index, 1)
  }

  const updateRelationshipAsync = async (communityId: string, userId: string) => {
    if (loadedCommunityRelationships.value[communityId] === undefined) return
    const response = await communityRelationshipsApi.getById(communityId, userId)
    if (!response.isSuccess()) return
    const index = loadedCommunityRelationships.value[communityId]?.findIndex(
      (relationship) => relationship.userId === userId
    )
    if (index === -1) {
      addRelationship(communityId, response.data!)
      return
    }
    loadedCommunityRelationships.value[communityId].splice(index, 1, response.data!)
  }

  return {
    loadCommunitiesAsync,
    communities,
    relatedCommunities,
    organizableCommunities,
    performableCommunities,
    getCommunity,
    removeCommunity,
    addCommunity,
    updateCommunityAsync,
    loadRelationshipsAsync,
    getRelationships,
    addRelationship,
    removeRelationship,
    updateRelationshipAsync
  }
})

type RelationshipMap = Record<string, CommunityRelationshipResponse[] | undefined>
type RelationshipPromiseMap = Record<string, Promise<void> | undefined>
