<template>
  <div class="new-audience-tab">
    <AudienceForm
      ref="createForm"
      :initial-state="createFormInitialState"
      :disabled="isSubmitting" />
    <Button
      class="input"
      label="Create"
      @click="handleCreateAudience"
      :disabled="isSubmitting" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceRequest } from '@/api/audiences/audienceRequest'
import AudienceForm from '@/components/form/requestForms/AudienceForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { ref, useTemplateRef } from 'vue'

const toast = useToast()
const isSubmitting = ref(false)

const emit = defineEmits<{
  createdAudience: []
}>()

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
  emit('createdAudience')
  createForm.value.reset()
}
</script>
