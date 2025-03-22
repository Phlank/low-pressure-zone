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
          input-id="selectCommunityInput" />
      </IftaFormField>
    </FormArea>
    <CommunityRelationshipsGrid
      :community="selectedCommunity"
      :relationships="relationships"
      @update="handleRelationshipUpdate" />
  </div>
</template>

<script lang="ts" setup>
import { Select } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { computed, inject, onMounted, ref, type Ref } from 'vue'
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
