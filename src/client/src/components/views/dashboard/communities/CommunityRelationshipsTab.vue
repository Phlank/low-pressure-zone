<template>
  <div class="community-relationships">
    <FormArea>
      <IftaFormField
        input-id="selectCommunityInput"
        label="Community"
        size="m">
        <Select
          v-model:model-value="selectedCommunity"
          :option-label="(data: CommunityResponse) => data.name"
          :option-value="(data: CommunityResponse) => data"
          :options="communities.organizableCommunities"
          input-id="selectCommunityInput" />
      </IftaFormField>
    </FormArea>
    <CommunityRelationshipsGrid
      v-if="!communities.isLoading"
      :community="selectedCommunity!"
      @create="handleCreate"
      @edit="handleEdit" />
    <Skeleton
      v-else
      style="height: 300px" />
    <FormDrawer
      v-model:visible="showRelationshipForm"
      :is-submitting="communityRelationshipFormRef?.isSubmitting"
      :title="editingRelationship ? 'Edit Relationship' : 'Add Relationship'"
      @reset="communityRelationshipFormRef?.reset()"
      @submit="communityRelationshipFormRef?.submit()">
      <CommunityRelationshipForm
        ref="communityRelationshipFormRef"
        :community-id="selectedCommunity?.id ?? ''"
        :relationship="editingRelationship"
        @submitted="showRelationshipForm = false"
        hide-actions />
    </FormDrawer>
  </div>
</template>

<script lang="ts" setup>
import { Select, Skeleton } from 'primevue'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import { ref, type Ref, useTemplateRef, watch } from 'vue'
import CommunityRelationshipsGrid from '@/components/views/dashboard/communities/CommunityRelationshipsGrid.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import CommunityRelationshipForm from '@/components/form/requestForms/CommunityRelationshipForm.vue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'

const communities = useCommunityStore()

const selectedCommunity: Ref<CommunityResponse | undefined> = ref(undefined)
watch(
  () => communities.organizableCommunities,
  (newVal) => {
    if (selectedCommunity.value === undefined && newVal.length > 0) {
      selectedCommunity.value = newVal[0]
    }
  },
  { immediate: true }
)

const showRelationshipForm = ref(false)
const editingRelationship: Ref<undefined | CommunityRelationshipResponse> = ref(undefined)
const communityRelationshipFormRef = useTemplateRef('communityRelationshipFormRef')
const handleCreate = () => {
  editingRelationship.value = undefined
  showRelationshipForm.value = true
}
const handleEdit = (relationship: CommunityRelationshipResponse) => {
  editingRelationship.value = relationship
  showRelationshipForm.value = true
}
</script>
