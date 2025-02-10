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
      <Column header="Actions">
        <template #body="slotProps">
          <Button
            icon="pi pi-pencil"
            @click="handleEditClick(slotProps.data as PerformerResponse)"
            rounded
            :outlined="true"
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
      <div class="desktop-inline-form">
        <IftaLabel class="input">
          <InputText
            id="name"
            :value="editForm?.name"
            @update:model-value="handleNameUpdate"
            :disabled="isSubmitting"
            :invalid="!validation?.isValid('name')"
          />
          <ValidationLabel for="name" :message="validation?.message('name')">Name</ValidationLabel>
        </IftaLabel>
        <IftaLabel class="input">
          <InputText
            id="url"
            class="input__large-field"
            :value="editForm?.url"
            @update:model-value="handleUrlUpdate"
            :disabled="isSubmitting"
            :invalid="!validation?.isValid('url')"
          />
          <ValidationLabel for="url" :message="validation?.message('url')">URL</ValidationLabel>
        </IftaLabel>
      </div>
    </FormDialog>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import FormDialog from '@/components/form/FormDialog.vue'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { showApiStatusToast, showCreateSuccessToast } from '@/utils/toastUtils'
import { nameValidator, urlValidator } from '@/validation/rules/composed/performerValidators'
import { createFormValidation } from '@/validation/types/formValidation'
import { Column, DataTable, Button, IftaLabel, InputText } from 'primevue'
import { onMounted, reactive, ref, type Ref } from 'vue'

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
const editForm: PerformerRequest = reactive({ name: '', url: '' })
const validation = createFormValidation(editForm, {
  name: nameValidator,
  url: urlValidator
})
const isSubmitting = ref(false)

const handleEditClick = (performer: PerformerResponse) => {
  editingId = performer.id
  editForm.name = performer.name
  editForm.url = performer.url
  validation.reset()
  showEditDialog.value = true
}

const handleNameUpdate = (newName?: string) => {
  if (newName == undefined) return
  editForm!.name = newName
  validation?.validateIfDirty('name')
}

const handleUrlUpdate = (newUrl?: string) => {
  if (newUrl == undefined) return
  editForm!.url = newUrl
  validation?.validateIfDirty('url')
}

const closeDialog = () => {
  showEditDialog.value = false
}

const handleSave = async () => {
  if (!validation?.validate()) {
    return
  }

  isSubmitting.value = true
  const response = await api.performers.put(editingId!, editForm!)
  isSubmitting.value = false

  if (!response.isSuccess()) {
    const errors = response.getValidationErrors()
    if (errors) {
      validation.mapApiValidationErrors(errors)
    } else {
      showApiStatusToast(response.status)
    }
    return
  }

  showCreateSuccessToast('performer', editForm!.name)
  closeDialog()
}
</script>
