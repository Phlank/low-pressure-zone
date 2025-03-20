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
      :available-users="availableUsers"
      :community="selectedCommunity"
      :relationships="relationships"
      @update="handleGridUpdate" />
  </div>
</template>

<script lang="ts" setup>
import { Divider, IftaLabel, Select, useToast } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { computed, type ComputedRef, onMounted, ref, type Ref, watch } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'
import type { UserResponse } from '@/api/resources/usersApi.ts'

const toast = useToast()

const props = defineProps<{
  communities: CommunityResponse[]
  users: UserResponse[]
}>()

const availableCommunities = computed(() =>
  props.communities.filter((community) => community.isOrganizable)
)
const selectedCommunity: Ref<CommunityResponse> = ref(availableCommunities.value[0])
const relationships: Ref<CommunityRelationshipResponse[]> = ref([])

const availableUsers: ComputedRef<UserResponse[]> = computed(() => {
  if (props.users.length === 0) return []
  const userIdsInUse = relationships.value.map((relationship) => relationship.userId)
  console.log(JSON.stringify(props.users))
  console.log(JSON.stringify(userIdsInUse))
  return props.users.filter((user) => userIdsInUse.indexOf(user.id) === -1)
})
const selectedUser: Ref<UserResponse | undefined> = ref(undefined)

onMounted(async () => {
  relationships.value = (await communityRelationshipsApi.get(selectedCommunity.value.id)).data!
})

const handleGridUpdate = async () => {
  relationships.value = (await communityRelationshipsApi.get(selectedCommunity.value.id)).data!
}

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
  availableUsers,
  (newValue) => {
    if (newValue.length === 0) selectedUser.value = undefined
    else selectedUser.value = newValue[0]
  },
  { immediate: true }
)
</script>
