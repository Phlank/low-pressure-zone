<template>
  <div class="desktop-inline-form">
    <IftaLabel class="input">
      <InputText
        id="name"
        :value="formState.name"
        @update:model-value="handleNameUpdate"
        :disabled="isSubmitting"
        :invalid="!validation.isValid('name')"
      />
      <ValidationLabel for="name" :message="validation.message('name')">Name</ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input">
      <InputText
        id="url"
        :value="formState.url"
        @update:model-value="handleUrlUpdate"
        :disabled="isSubmitting"
        :invalid="!validation.isValid('url')"
      />
      <ValidationLabel for="url" :message="validation.message('url')">URL</ValidationLabel>
    </IftaLabel>
    <Button class="input" label="Create" @click="handleCreatePerformer" :disabled="isSubmitting" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { showApiStatusToast, showCreateSuccessToast } from '@/utils/toastUtils'
import { nameValidator, urlValidator } from '@/validation/rules/composed/performerValidators'
import { FormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText, Button } from 'primevue'
import { reactive, ref } from 'vue'

const isSubmitting = ref(false)
const formState = reactive({ name: '', url: '' } as PerformerRequest)
const validation = new FormValidation(formState, {
  name: nameValidator,
  url: urlValidator
})

const handleNameUpdate = (value: string | undefined) => {
  formState.name = value ?? ''
  validation.validateIfDirty('name')
}

const handleUrlUpdate = (value: string | undefined) => {
  formState.url = value ?? ''
  validation.validateIfDirty('url')
}

const handleCreatePerformer = async () => {
  validation.validate()
  if (!validation.isValid()) {
    return
  }

  isSubmitting.value = true
  const response = await api.performers.post(formState)
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

  showCreateSuccessToast('performer', formState.name)
  formState.name = ''
  formState.url = ''
  validation.reset()
}
</script>
