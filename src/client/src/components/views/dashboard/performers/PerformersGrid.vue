<template>
  <div class="performer-grid">
    <DataTable v-if="isLoaded" :value="performers" data-key="id">
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
            @click="handleEditClick(slotProps.data as PerformerResponse)"
            rounded
            outlined
          />
          <Button
            class="action"
            icon="pi pi-trash"
            @click="handleDeleteClick(slotProps.data as PerformerResponse)"
            rounded
            outlined
          />
        </template>
      </Column>
    </DataTable>
    <FormDialog
      :visible="showEditDialog"
      title="Edit Performer"
      :is-submitting="isSubmitting"
      @close="closeDialog"
      @save="handleSave"
    >
      <PerformerForm
        ref="editForm"
        :initial-state="initialFormData"
        :is-submitting="isSubmitting"
      />
    </FormDialog>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showApiStatusToast, showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, Column, DataTable } from 'primevue'
import { onMounted, ref, useTemplateRef, type Ref } from 'vue'

const isLoaded = ref(false)
const performers: Ref<PerformerResponse[]> = ref([])

onMounted(async () => {
  const response = await api.performers.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
})

const showEditDialog = ref(false)
let editingId = ''
const editForm = useTemplateRef('editForm')
const initialFormData: Ref<PerformerRequest> = ref({ name: '', url: '' })
const isSubmitting = ref(false)

const handleEditClick = (performer: PerformerResponse) => {
  editingId = performer.id
  initialFormData.value.name = performer.name
  initialFormData.value.url = performer.url
  showEditDialog.value = true
}

const handleDeleteClick = (performer: PerformerResponse) => {}

const closeDialog = () => {
  showEditDialog.value = false
}

const handleSave = async () => {
  editForm.value?.validation.validate()
  if (!editForm.value?.validation.isValid()) {
    return
  }

  isSubmitting.value = true
  const response = await api.performers.put(editingId, editForm.value.formState)
  isSubmitting.value = false

  if (!response.isSuccess()) {
    const errors = response.getValidationErrors()
    if (errors) {
      editForm.value.validation.mapApiValidationErrors(errors)
    } else {
      showApiStatusToast(response.status)
    }
    return
  }

  showCreateSuccessToast('performer', initialFormData.value.name)
  closeDialog()
}
</script>
