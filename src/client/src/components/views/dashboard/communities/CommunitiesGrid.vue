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
      <FormDialog
        :is-submitting="isSubmitting"
        :visible="showEditDialog"
        header="Edit Community"
        @close="showEditDialog = false"
        @save="handleSave">
        <CommunityForm
          ref="editForm"
          :disabled="isSubmitting"
          :initial-state="editFormInitialState" />
      </FormDialog>
      <DeleteDialog
        :entity-name="deletingName"
        :is-submitting="isSubmitting"
        :visible="showDeleteDialog"
        entity-type="community"
        header="Delete Community"
        @close="showDeleteDialog = false"
        @delete="handleDelete" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import CommunityForm from '@/components/form/requestForms/CommunityForm.vue'
import { showDeleteSuccessToast, showEditSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { inject, ref, type Ref, useTemplateRef } from 'vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import communitiesApi, {
  type CommunityRequest,
  type CommunityResponse
} from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const isSubmitting = ref(false)
const toast = useToast()

defineProps<{
  communities: CommunityResponse[]
}>()

const emit = defineEmits<{
  edited: [id: string]
  deleted: [id: string]
}>()

const showEditDialog = ref(false)
let editingId = ''
const editFormInitialState: Ref<CommunityRequest> = ref({ name: '', url: '' })
const editForm = useTemplateRef('editForm')

const handleEditActionClick = (community: CommunityResponse) => {
  editingId = community.id
  editFormInitialState.value.name = community.name
  editFormInitialState.value.url = community.url
  showEditDialog.value = true
}
const handleSave = async () => {
  if (!editForm.value) return

  const isValid = editForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communitiesApi.put(editingId, editForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, editForm.value.validation)) return

  showEditSuccessToast(toast, 'community', editForm.value.formState.name)
  showEditDialog.value = false
  emit('edited', editingId)
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
  emit('deleted', deletingId)
}
</script>
