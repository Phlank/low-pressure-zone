<template>
  <div class="performer-form">
    <FormArea :align-actions="alignActions">
      <IftaFormField
        :message="validation.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="formState.name"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('name')"
          autofocus
          @update:model-value="validation.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('url')"
        input-id="urlInput"
        label="URL"
        size="l">
        <InputText
          id="urlInput"
          v-model:model-value="formState.url"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('url')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { createFormValidation } from '@/validation/types/formValidation'
import { Button, InputText, useToast } from 'primevue'
import { onMounted, type Ref, ref } from 'vue'
import { performerRequestRules } from '@/validation/requestRules'
import performersApi, { type PerformerRequest } from '@/api/resources/performersApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import type { ApiResponse } from '@/api/apiResponse.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'

const formState: Ref<PerformerRequest> = ref({ name: '', url: '' })
const validation = createFormValidation(formState, performerRequestRules)
const toast = useToast()
const performerStore = usePerformerStore()
const isSubmitting = ref(false)

const props = withDefaults(
  defineProps<{
    performerId?: string
    initialState?: PerformerRequest
    alignActions?: 'left' | 'right'
  }>(),
  {
    performerId: undefined,
    initialState: undefined,
    alignActions: 'left'
  }
)

onMounted(() => {
  reset()
})

const reset = () => {
  formState.value.name = props.initialState?.name ?? ''
  formState.value.url = props.initialState?.url ?? ''
  validation.reset()
}

const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  let response: ApiResponse<PerformerRequest, never>
  if (props.performerId === '' || props.performerId === undefined) {
    response = await performersApi.post(formState.value)
  } else {
    response = await performersApi.put(props.performerId, formState.value)
  }
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return
  showSuccessToast(
    toast,
    props.performerId ? 'Updated' : 'Created',
    'Performer',
    formState.value.name
  )
  await performerStore.loadPerformersAsync()
  emits('afterSubmit')
  reset()
}

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
