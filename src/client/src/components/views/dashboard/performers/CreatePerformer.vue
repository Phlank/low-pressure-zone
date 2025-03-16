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
import performersApi, { type PerformerRequest } from '@/api/resources/performersApi.ts'

const toast = useToast()
const createFormInitialState: PerformerRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const controlsDisabled = computed(() => false)

const isSubmitting = ref(false)
const handleCreatePerformer = async () => {
  if (!createForm.value) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await performersApi.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return

  showCreateSuccessToast(toast, 'performer', createForm.value.formState.name)
  emit('createdPerformer')
  createForm.value.reset()
}

const emit = defineEmits<{
  createdPerformer: []
}>()
</script>
