<template>
  <div class="community-relationships">
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
    <CommunityRelationshipsGrid :relationships="relationships" />
  </div>
</template>

<script lang="ts" setup>
import { IftaLabel, Select, useToast } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { computed, ref, type Ref, watch } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'

const toast = useToast()

const props = defineProps<{
  communities: CommunityResponse[]
}>()

const availableCommunities = computed(() =>
  props.communities.filter((community) => community.isOrganizable)
)
const selectedCommunity: Ref<CommunityResponse> = ref(availableCommunities.value[0])
const relationships: Ref<CommunityRelationshipResponse[]> = ref([])

watch(selectedCommunity, async (newCommunity) => {
  const response = await communityRelationshipsApi.get(newCommunity.id)
  if (tryHandleUnsuccessfulResponse(response, toast)) return
  relationships.value = response.data!
})
</script>
