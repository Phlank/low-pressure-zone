<template>
  <div class="audiences-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="audiences"
        data-key="id">
        <Column
          field="name"
          header="Name" />
        <Column
          field="url"
          header="URL" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: AudienceResponse }">
            <GridActions
              :show-edit="data.isEditable"
              :show-delete="data.isDeletable"
              @edit="handleEditActionClick(data)"
              @delete="handleDeleteActionClick(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <ListItem
        v-for="audience in audiences"
        :key="audience.id">
        <template #left>
          {{ audience.name }}
        </template>
        <template #right>
          <GridActions
            :show-edit="audience.isEditable"
            :show-delete="audience.isDeletable"
            @edit="handleEditActionClick(audience)"
            @delete="handleDeleteActionClick(audience)" />
        </template>
      </ListItem>
    </div>
    <div class="audience-grid__dialogs">
      <FormDialog
        :visible="showEditDialog"
        header="Edit Audience"
        :is-submitting="isSubmitting"
        @close="showEditDialog = false"
        @save="handleSave">
        <AudienceForm
          ref="editForm"
          :initial-state="editFormInitialState"
          :disabled="isSubmitting" />
      </FormDialog>
      <DeleteDialog
        entity-type="audience"
        :entity-name="deletingName"
        header="Delete Audience"
        :is-submitting="isSubmitting"
        :visible="showDeleteDialog"
        @close="showDeleteDialog = false"
        @delete="handleDelete" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceRequest } from '@/api/audiences/audienceRequest'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import AudienceForm from '@/components/form/requestForms/AudienceForm.vue'
import { showDeleteSuccessToast, showEditSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { inject, ref, useTemplateRef, type Ref } from 'vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import { throwIfUndefined } from '@/utils/guards'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const isSubmitting = ref(false)
const toast = useToast()

defineProps<{
  audiences: AudienceResponse[]
}>()

const showEditDialog = ref(false)
let editingId = ''
const editFormInitialState: Ref<AudienceRequest> = ref({ name: '', url: '' })
const editForm = useTemplateRef('editForm')

const handleEditActionClick = (audience: AudienceResponse) => {
  editingId = audience.id
  editFormInitialState.value.name = audience.name
  editFormInitialState.value.url = audience.url
  showEditDialog.value = true
}
const handleSave = async () => {
  if (!editForm.value) return

  const isValid = editForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.performers.put(editingId, editForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, editForm.value.validation)) return

  showEditSuccessToast(toast, 'audience', editFormInitialState.value.name)
  showEditDialog.value = false
}

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteActionClick = (performer: AudienceResponse) => {
  deletingId = performer.id
  deletingName.value = performer.name
  showDeleteDialog.value = true
}
const handleDelete = async () => {
  isSubmitting.value = true
  const response = await api.audiences.delete(deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  showDeleteSuccessToast(toast, 'audience', deletingName.value)
  showDeleteDialog.value = false
}
</script>
