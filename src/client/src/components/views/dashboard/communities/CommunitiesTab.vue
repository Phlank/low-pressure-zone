<template>
  <div class="communities-tab">
    <CommunitiesGrid
      v-if="!communities.isLoading"
      :communities="communities.relatedCommunities"
      @create="handleCreate"
      @delete="handleDeleteAction"
      @edit="handleEdit" />
    <Skeleton
      v-else
      style="height: 300px" />
    <FormDrawer
      v-model:visible="showCommunityForm"
      :is-submitting="communityFormRef?.isSubmitting"
      :title="editingCommunity ? `Edit Community - ${editingCommunity.name}` : 'Create Community'"
      @reset="communityFormRef?.reset()"
      @submit="communityFormRef?.submit()">
      <CommunityForm
        ref="communityFormRef"
        :community="editingCommunity"
        hide-actions
        @submitted="showCommunityForm = false" />
    </FormDrawer>
    <DeleteDialog
      v-if="deletingCommunity"
      v-model:visible="showDeleteDialog"
      :entity-name="deletingCommunity.name"
      :is-submitting="isDeleting"
      entity-type="Community"
      header="Delete Community"
      @delete="handleDeleteConfirm" />
  </div>
</template>

<script lang="ts" setup>
import CommunitiesGrid from '@/components/views/dashboard/communities/CommunitiesGrid.vue'
import {useCommunityStore} from '@/stores/communityStore.ts'
import FormDrawer from '@/components/form/FormDrawer.vue'
import CommunityForm from '@/components/form/requestForms/CommunityForm.vue'
import {type Ref, ref, useTemplateRef} from 'vue'
import type {CommunityResponse} from '@/api/resources/communitiesApi.ts'
import {Skeleton} from 'primevue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'

const communities = useCommunityStore()

const showCommunityForm = ref(false)
const editingCommunity: Ref<CommunityResponse | undefined> = ref(undefined)
const communityFormRef = useTemplateRef('communityFormRef')
const handleCreate = () => {
  console.log('Create Community')
  editingCommunity.value = undefined
  showCommunityForm.value = true
}
const handleEdit = (community: CommunityResponse) => {
  console.log('Edit Community')
  editingCommunity.value = community
  showCommunityForm.value = true
}

const showDeleteDialog = ref(false)
const deletingCommunity: Ref<CommunityResponse | undefined> = ref(undefined)
const isDeleting = ref(false)
const handleDeleteAction = (community: CommunityResponse) => {
  deletingCommunity.value = community
  showDeleteDialog.value = true
}
const handleDeleteConfirm = async () => {
  isDeleting.value = true
  const result = await communities.removeCommunity(deletingCommunity.value!.id)
  isDeleting.value = false
  if (!result.isSuccess) return
  showDeleteDialog.value = false
  deletingCommunity.value = undefined
}
</script>
