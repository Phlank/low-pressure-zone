<template>
  <div class="performers-dashboard">
    <div class="desktop-inline">
      <PerformerForm
        ref="createForm"
        :initial-state="createFormInitialState"
        :disabled="controlsDisabled"
      />
      <Button
        class="input"
        label="Create"
        @click="handleCreatePerformer"
        :disabled="controlsDisabled"
      />
    </div>
    <DataTable :loading="!isLoaded" :value="performers" data-key="id">
      <Column field="name" header="Name" />
      <Column field="url" header="URL" />
      <Column field="modifiedDate" header="Last Modified">
        <template #body="slotProps">
          {{ new Date(slotProps.data.modifiedDate).toLocaleString() }}
        </template>
      </Column>
      <Column class="grid-action-col">
        <template #body="slotProps">
          <Button
            class="grid-action-col__item"
            icon="pi pi-pencil"
            severity="secondary"
            @click="handleEditActionClick(slotProps.data as PerformerResponse)"
            :disabled="controlsDisabled"
            rounded
            outlined
          />
          <Button
            v-if="slotProps.data.canDelete"
            class="grid-action-col__item"
            icon="pi pi-trash"
            severity="danger"
            @click="handleDeleteActionClick(slotProps.data as PerformerResponse)"
            :disabled="controlsDisabled"
            rounded
            outlined
          />
        </template>
      </Column>
    </DataTable>
    <FormDialog
      :visible="showEditDialog"
      header="Edit Performer"
      :is-submitting="isSubmitting"
      @close="showEditDialog = false"
      @save="handleSave"
    >
      <PerformerForm
        ref="editForm"
        :initial-state="editFormInitialState"
        :disabled="isSubmitting"
      />
    </FormDialog>
    <DeleteDialog
      entity-type="performer"
      :entity-name="deletingName"
      header="Delete Performer"
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
import type { PerformerRequest } from '@/api/performers/performerRequest'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { Button, Column, DataTable, useToast } from 'primevue'
import { computed, onMounted, ref, useTemplateRef, type Ref } from 'vue'

const isLoaded = ref(false)
const isSubmitting = ref(false)
const performers: Ref<PerformerResponse[]> = ref([])
const toast = useToast()
const controlsDisabled = computed(
  () => !isLoaded.value || isSubmitting.value || showEditDialog.value || showDeleteDialog.value
)

onMounted(async () => {
  await loadPerformers()
})

const loadPerformers = async () => {
  const response = await api.performers.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
}

// CREATING PERFORMERS

const createFormInitialState: PerformerRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const handleCreatePerformer = async () => {
  if (createForm.value == undefined) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.performers.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return

  showCreateSuccessToast(toast, 'performer', createForm.value.formState.name)
  await loadPerformers()
  createForm.value.reset()
}

// EDITING PERFORMERS

const showEditDialog = ref(false)
let editingId = ''
const editFormInitialState: Ref<PerformerRequest> = ref({ name: '', url: '' })
const editForm = useTemplateRef('editForm')

const handleEditActionClick = (performer: PerformerResponse) => {
  editingId = performer.id
  editFormInitialState.value.name = performer.name
  editFormInitialState.value.url = performer.url
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
  const performerInGrid = performers.value.find((performer) => performer.id == editingId)
  if (performerInGrid) {
    performerInGrid.name = editForm.value.formState.name
    performerInGrid.url = editForm.value.formState.url
    performerInGrid.modifiedDate = new Date().toLocaleString()
  }
  showEditDialog.value = false
}

// DELETING PERFORMERS

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')

const handleDeleteActionClick = (performer: PerformerResponse) => {
  deletingId = performer.id
  deletingName.value = performer.name
  showDeleteDialog.value = true
}

const handleDelete = async () => {
  isSubmitting.value = true
  const response = await api.performers.delete(deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  const performerInGrid = performers.value.find((performer) => performer.id == deletingId)
  performers.value.splice(performers.value.indexOf(performerInGrid!), 1)
  showDeleteSuccessToast(toast, 'performer', deletingName.value)
  showDeleteDialog.value = false
}
</script>
