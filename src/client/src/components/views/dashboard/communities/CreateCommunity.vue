<template>
  <div class="new-community-tab">
    <CommunityForm
      ref="createForm"
      :disabled="isSubmitting"
      :initial-state="createFormInitialState" />
    <Button
      :disabled="isSubmitting"
      class="input"
      label="Create"
      @click="handleCreateCommunity" />
  </div>
</template>

<script lang="ts" setup>
import CommunityForm from '@/components/form/requestForms/CommunityForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { ref, useTemplateRef } from 'vue'
import communitiesApi, { type CommunityRequest } from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const toast = useToast()
const isSubmitting = ref(false)

const emit = defineEmits<{
  createdCommunity: []
}>()

const createFormInitialState: CommunityRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const handleCreateCommunity = async () => {
  if (!createForm.value) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communitiesApi.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return

  showCreateSuccessToast(toast, 'performer', createForm.value.formState.name)
  emit('createdCommunity')
  createForm.value.reset()
}
</script>
