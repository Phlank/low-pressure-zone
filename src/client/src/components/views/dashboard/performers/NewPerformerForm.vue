<template>
  <div class="desktop-inline-form">
    <Toast />
    <PerformerForm ref="form" :initial-state="initialState" :is-submitting="isSubmitting" />
    <Button class="input" label="Create" @click="handleCreatePerformer" :disabled="isSubmitting" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showApiStatusToast, showCreateSuccessToast } from '@/utils/toastUtils'
import { Button } from 'primevue'
import { ref, useTemplateRef } from 'vue'

const isSubmitting = ref(false)
const initialState: PerformerRequest = { name: '', url: '' }
const form = useTemplateRef('form')

const handleCreatePerformer = async () => {
  if (!form.value?.validation.validate()) return

  isSubmitting.value = true
  const response = await api.performers.post(form.value!.formState)
  isSubmitting.value = false

  if (!response.isSuccess()) {
    const errors = response.getValidationErrors()
    if (errors) {
      form.value!.validation.mapApiValidationErrors(errors)
    } else {
      showApiStatusToast(response.status)
    }
    return
  }

  showCreateSuccessToast('performer', form.value!.formState.name)
  form.value.reset()
}
</script>
