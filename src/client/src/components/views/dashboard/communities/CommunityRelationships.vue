<template>
  <div class="community-relationships">
    <FormArea>
      <IftaFormField
        :size="isMobile ? 'm' : 's'"
        input-id="selectCommunityInput"
        label="Community">
        <Select
          v-model:model-value="selectedCommunity"
          :option-label="(data: CommunityResponse) => data.name"
          :option-value="(data: CommunityResponse) => data"
          :options="availableCommunities"
          input-id="selectCommunityInput"
          @update:model-value="handleCommunityChange" />
      </IftaFormField>
    </FormArea>
    <CommunityRelationshipsGrid :community="selectedCommunity" />
  </div>
</template>

<script lang="ts" setup>
import { Select } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { inject, onMounted, ref, type Ref } from 'vue'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const communityStore = useCommunityStore()

const props = defineProps<{
  availableCommunities: CommunityResponse[]
}>()

const selectedCommunity: Ref<CommunityResponse> = ref(props.availableCommunities[0])

onMounted(async () => {
  if (communityStore.getRelationships(selectedCommunity.value.id).length === 0) {
    await communityStore.loadRelationshipsAsync(selectedCommunity.value.id)
  }
})

const handleCommunityChange = async (newCommunity: CommunityResponse) => {
  if (communityStore.getRelationships(newCommunity.id).length === 0) {
    await communityStore.loadRelationshipsAsync(newCommunity.id)
  }
}
</script>
