<template>
  <div class="create-performer">
    <h4>Create New Performer</h4>
    <div class="desktop-inline">
      <PerformerForm
        ref="createForm"
        :initial-state="createFormInitialState"
        :disabled="controlsDisabled" />
      <Button
        class="input"
        label="Create"
        @click="handleCreatePerformer"
        :disabled="controlsDisabled" />
    </div>
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { PerformerRequest } from '@/api/performers/performerRequest'
import PerformerForm from '@/components/form/requestForms/PerformerForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { computed, ref, useTemplateRef } from 'vue'

const toast = useToast()
const createFormInitialState: PerformerRequest = { name: '', url: '' }
const createForm = useTemplateRef('createForm')

const controlsDisabled = computed(() => false)

const isSubmitting = ref(false)
const handleCreatePerformer = async () => {
  if (createForm.value == undefined) return
  const isValid = createForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.performers.post(createForm.value.formState)
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
