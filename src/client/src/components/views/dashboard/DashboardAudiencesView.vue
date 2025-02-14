<template>
  <div class="performers-dashboard">
    <div class="create-form desktop-inline">
      <AudienceForm
        ref="createForm"
        :initial-state="createFormInitialState"
        :disabled="controlsDisabled"
      />
      <Button
        class="input"
        label="Create"
        @click="handleCreateAudience"
        :disabled="controlsDisabled"
      />
    </div>
    <DataTable v-if="isLoaded" :value="audiences" data-key="id">
      <Column field="name" header="Name" />
      <Column field="url" header="URL" />
      <Column field="modifiedDate" header="Last Modified">
        <template #body="slotProps">
          {{ new Date(slotProps.data.modifiedDate).toLocaleString() }}
        </template>
      </Column>
      <Column style="text-align: right">
        <template #body="slotProps">
          <Button
            class="action"
            icon="pi pi-pencil"
            @click="handleEditActionClick(slotProps.data as AudienceResponse)"
            :disabled="controlsDisabled"
            rounded
            outlined
          />
          <Button
            v-if="slotProps.data.canDelete"
            class="action"
            icon="pi pi-trash"
            severity="danger"
            @click="handleDeleteActionClick(slotProps.data as AudienceResponse)"
            :disabled="controlsDisabled"
            rounded
            outlined
          />
        </template>
      </Column>
    </DataTable>
    <FormDialog
      :visible="showEditDialog"
      header="Edit Audience"
      :is-submitting="isSubmitting"
      @close="showEditDialog = false"
      @save="handleSave"
    >
      <AudienceForm ref="editForm" :initial-state="editFormInitialState" :disabled="isSubmitting" />
    </FormDialog>
    <DeleteDialog
      entity-type="audience"
      :entity-name="deletingName"
      header="Delete Audience"
      :is-submitting="isSubmitting"
      :visible="showDeleteDialog"
      @close="showDeleteDialog = false"
      @delete="handleDelete"
    />
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
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { Button, Column, DataTable, useToast } from 'primevue'
import { computed, onMounted, ref, useTemplateRef, type Ref } from 'vue'

const isLoaded = ref(false)
const isSubmitting = ref(false)
const audiences: Ref<AudienceResponse[]> = ref([])
const toast = useToast()
const controlsDisabled = computed(
  () => !isLoaded.value || isSubmitting.value || showEditDialog.value || showDeleteDialog.value
)

onMounted(async () => {
  await loadAudiences()
})

const loadAudiences = async () => {
  const response = await api.audiences.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  audiences.value = response.data!
}

// CREATING PERFORMERS

const createFormInitialState: AudienceRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const handleCreateAudience = async () => {
  if (createForm.value == undefined) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.audiences.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return

  showCreateSuccessToast(toast, 'performer', createForm.value.formState.name)
  await loadAudiences()
  createForm.value.reset()
}

// EDITING PERFORMERS

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
  if (editForm.value == undefined) return
  const isValid = editForm.value.validation.validate()
  if (!isValid) return

  if (
    editForm.value.formState.name === editFormInitialState.value.name &&
    editForm.value.formState.url === editFormInitialState.value.url
  ) {
    showEditDialog.value = false
    return
  }

  isSubmitting.value = true
  const response = await api.performers.put(editingId, editForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, editForm.value.validation)) return

  showEditSuccessToast(toast, 'performer', editFormInitialState.value.name)
  const audienceInGrid = audiences.value.find((audience) => audience.id == editingId)
  if (audienceInGrid) {
    audienceInGrid.name = editForm.value.formState.name
    audienceInGrid.url = editForm.value.formState.url
    audienceInGrid.modifiedDate = new Date().toLocaleString()
  }
  showEditDialog.value = false
}

// DELETING PERFORMERS

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

  const performerInGrid = audiences.value.find((performer) => performer.id == deletingId)
  audiences.value.splice(audiences.value.indexOf(performerInGrid!), 1)
  showDeleteSuccessToast(toast, 'performer', deletingName.value)
  showDeleteDialog.value = false
}
</script>
