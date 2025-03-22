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
import communitiesApi, {
  type CommunityRequest,
  type CommunityResponse
} from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'

const toast = useToast()
const communityStore = useCommunityStore()
const isSubmitting = ref(false)

const createFormInitialState: CommunityRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const handleCreateCommunity = async () => {
  if (!createForm.value) return
  const formState = createForm.value.formState
  const validation = createForm.value.validation
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await communitiesApi.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  showCreateSuccessToast(toast, 'performer', formState.name)
  const newCommunity: CommunityResponse = {
    id: response.getCreatedId()!,
    name: formState.name,
    url: formState.url,
    isOrganizable: true,
    isPerformable: true,
    isDeletable: true,
    isEditable: true
  }
  communityStore.addCommunity(newCommunity)
  createForm.value.reset()
}
</script>
