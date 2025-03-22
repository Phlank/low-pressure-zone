<template>
  <div class="create-performer">
    <h4>Create New Performer</h4>
    <div class="desktop-inline">
      <PerformerForm
        ref="createForm"
        :disabled="controlsDisabled"
        :initial-state="createFormInitialState" />
      <Button
        :disabled="controlsDisabled"
        class="input"
        label="Create"
        @click="handleCreatePerformer" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { computed, ref, useTemplateRef } from 'vue'
import performersApi, {
  type PerformerRequest,
  type PerformerResponse
} from '@/api/resources/performersApi.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'

const toast = useToast()
const performerStore = usePerformerStore()
const createFormInitialState: PerformerRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const controlsDisabled = computed(() => false)

const isSubmitting = ref(false)
const handleCreatePerformer = async () => {
  if (!createForm.value) return
  const formState = createForm.value.formState
  const validation = createForm.value.validation

  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await performersApi.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  showCreateSuccessToast(toast, 'performer', formState.name)

  const newPerformer: PerformerResponse = {
    id: response.getCreatedId()!,
    name: formState.name,
    url: formState.url,
    isLinkableToTimeslot: true,
    isDeletable: true,
    isEditable: true
  }
  performerStore.add(newPerformer)
  createForm.value.reset()
}
</script>
