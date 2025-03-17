<template>
  <div class="community-relationships">
    Viewing relationships for:
    <IftaLabel class="input input--medium">
      <Select
        v-model:model-value="selectedCommunity"
        :option-label="(data: CommunityResponse) => data.name"
        :option-value="(data: CommunityResponse) => data"
        :options="availableCommunities"
        class="input__field"
        input-id="selectCommunityInput">
      </Select>
      <label for="selectCommunityInput">Community</label>
    </IftaLabel>
    <Divider />
    <CommunityRelationshipsGrid
      :relationships="relationships"
      :usernames="availableUsernames" />
  </div>
</template>

<script lang="ts" setup>
import { Divider, IftaLabel, Select, useToast } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { computed, ref, type Ref, watch } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'
import type { UsernameResponse } from '@/api/resources/usersApi.ts'

const toast = useToast()

const props = defineProps<{
  communities: CommunityResponse[]
  usernameResponses: UsernameResponse[]
}>()

const availableCommunities = computed(() =>
  props.communities.filter((community) => community.isOrganizable)
)
const selectedCommunity: Ref<CommunityResponse> = ref(availableCommunities.value[0])
const relationships: Ref<CommunityRelationshipResponse[]> = ref([])

const availableUsernames: ComputedRef<UsernameResponse[]> = computed(() => {
  if (props.usernameResponses.length === 0) return []
  const userIdsInUse = relationships.value.map((relationship) => relationship.userId)
  return props.usernameResponses.filter((response) => userIdsInUse.indexOf(response.id) === -1)
})
const selectedUsername: Ref<UsernameResponse | undefined> = ref(undefined)

const handleAddUserClick = async () => {}

watch(
  selectedCommunity,
  async (newCommunity) => {
    const relationshipsResponse = await communityRelationshipsApi.get(newCommunity.id)
    if (tryHandleUnsuccessfulResponse(relationshipsResponse, toast)) return
    relationships.value = relationshipsResponse.data!
  },
  { immediate: true }
)

watch(
  availableUsernames,
  (newValue) => {
    if (newValue.length === 0) selectedUsername.value = undefined
    else selectedUsername.value = newValue[0]
  },
  { immediate: true }
)
</script>
