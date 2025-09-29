<template>
  <div class="community-form">
    <FormArea :align-actions="alignActions">
      <IftaFormField
        :message="validation.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="formState.name"
          :disabled="disabled || isSubmitting"
          :invalid="!validation.isValid('name')"
          @update:model-value="validation.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('url')"
        input-id="urlInput"
        label="URL"
        size="m">
        <InputText
          id="urlInput"
          v-model:model-value="formState.url"
          :disabled="disabled || isSubmitting"
          :invalid="!validation.isValid('url')"
          @update:model-value="validation.validateIfDirty('url')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting || disabled"
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
import { communityRequestRules } from '@/validation/requestRules'
import communitiesApi, { type CommunityRequest } from '@/api/resources/communitiesApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'

const toast = useToast()
const communityStore = useCommunityStore()
const formState: Ref<CommunityRequest> = ref({ name: '', url: '' })
const validation = createFormValidation(formState, communityRequestRules)
const isSubmitting = ref(false)

const props = withDefaults(
  defineProps<{
    communityId?: string
    initialState?: CommunityRequest
    disabled?: boolean
    alignActions?: 'left' | 'right'
  }>(),
  {
    communityId: undefined,
    initialState: undefined,
    disabled: false,
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
  let result: ApiResponse<CommunityRequest, never>
  if (props.communityId === '' || props.communityId === undefined) {
    result = await communitiesApi.post(formState.value)
  } else {
    result = await communitiesApi.put(props.communityId, formState.value)
  }
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(result, toast, validation)) return
  showSuccessToast(
    toast,
    props.communityId ? 'Updated' : 'Created',
    'Community',
    formState.value.name
  )
  await communityStore.loadCommunitiesAsync()
  emits('afterSubmit')
  reset()
}

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
