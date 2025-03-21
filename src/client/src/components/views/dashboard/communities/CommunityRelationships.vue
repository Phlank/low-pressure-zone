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
      :community="selectedCommunity"
      :relationships="relationships"
      @update="handleRelationshipUpdate" />
  </div>
</template>

<script lang="ts" setup>
import { Divider, IftaLabel, Select } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { computed, onMounted, ref, type Ref } from 'vue'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'

const communityStore = useCommunityStore()

const props = defineProps<{
  availableCommunities: CommunityResponse[]
}>()

const selectedCommunity: Ref<CommunityResponse> = ref(props.availableCommunities[0])
const relationships = computed(() => communityStore.getRelationships(selectedCommunity.value.id))

onMounted(async () => {
  if (relationships.value.length === 0) {
    await communityStore.loadRelationshipsAsync(selectedCommunity.value.id)
  }
})

const handleRelationshipUpdate = async () => {
  await communityStore.loadRelationshipsAsync(selectedCommunity.value.id)
}
</script>
