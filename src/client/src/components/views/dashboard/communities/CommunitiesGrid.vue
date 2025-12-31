<template>
  <div class="communities-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="communities"
        data-key="id">
        <Column
          field="name"
          header="Name" />
        <Column
          field="url"
          header="URL" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: CommunityResponse }">
            <GridActions
              :show-delete="data.isDeletable"
              :show-edit="data.isEditable"
              @delete="handleDeleteActionClick(data)"
              @edit="handleEditActionClick(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <ListItem
        v-for="community in communities"
        :key="community.id">
        <template #left>
          {{ community.name }}
        </template>
        <template #right>
          <GridActions
            :show-delete="community.isDeletable"
            :show-edit="community.isEditable"
            @delete="handleDeleteActionClick(community)"
            @edit="handleEditActionClick(community)" />
        </template>
      </ListItem>
    </div>
    <div class="community-grid__dialogs">
      <Dialog
        v-model:visible="showEditDialog"
        :draggable="false"
        header="Edit Community"
        modal>
        <CommunityForm
          :community-id="editingId"
          :initial-state="editFormInitialState"
          align-actions="right"
          @after-submit="showEditDialog = false" />
      </Dialog>
      <DeleteDialog
        :entity-name="deletingName"
        :is-submitting="isSubmitting"
        :visible="showDeleteDialog"
        entity-type="community"
        header="Delete Community"
        @hide="showDeleteDialog = false"
        @delete="handleDelete" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import CommunityForm from '@/components/form/requestForms/CommunityForm.vue'
import { showDeleteSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, Dialog, useToast } from 'primevue'
import { inject, ref, type Ref } from 'vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import communitiesApi, {
  type CommunityRequest,
  type CommunityResponse
} from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const isSubmitting = ref(false)
const toast = useToast()
const communityStore = useCommunityStore()

defineProps<{
  communities: CommunityResponse[]
}>()

const showEditDialog = ref(false)
const editFormInitialState: Ref<CommunityRequest> = ref({ name: '', url: '' })
const editingId = ref('')

const handleEditActionClick = (community: CommunityResponse) => {
  editingId.value = community.id
  editFormInitialState.value = community
  showEditDialog.value = true
}

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteActionClick = (community: CommunityResponse) => {
  deletingId = community.id
  deletingName.value = community.name
  showDeleteDialog.value = true
}
const handleDelete = async () => {
  isSubmitting.value = true
  const response = await communitiesApi.delete(deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  showDeleteSuccessToast(toast, 'community', deletingName.value)
  showDeleteDialog.value = false
  communityStore.removeCommunity(deletingId)
}
</script>
