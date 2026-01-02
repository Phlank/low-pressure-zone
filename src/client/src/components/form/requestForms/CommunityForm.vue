<template>
  <div class="community-form">
    <FormArea>
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
        size="m">
        <InputText
          id="urlInput"
          v-model:model-value="formState.url"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('url')"
          @update:model-value="validation.validateIfDirty('url')" />
      </IftaFormField>
      <template #actions>
        <Button
          v-if="!hideActions"
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
import { Button, InputText } from 'primevue'
import { onMounted, type Ref, ref } from 'vue'
import { communityRequestRules } from '@/validation/requestRules'
import { type CommunityRequest, type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import type { Result } from '@/types/result.ts'

const communities = useCommunityStore()
const formState: Ref<CommunityRequest> = ref({ name: '', url: '' })
const validation = createFormValidation(formState, communityRequestRules)
const isSubmitting = ref(false)

const props = withDefaults(
  defineProps<{
    community?: CommunityResponse
    hideActions?: boolean
  }>(),
  {
    community: undefined,
    hideActions: false
  }
)

const submit = async () => {
  isSubmitting.value = true
  let result: Result
  if (props.community?.id)
    result = await communities.updateCommunity(props.community.id, formState, validation)
  else result = await communities.createCommunity(formState, validation)
  isSubmitting.value = false
  if (!result.isSuccess) return
  reset()
  emit('submitted')
}

const reset = () => {
  formState.value.name = props.community?.name ?? ''
  formState.value.url = props.community?.url ?? ''
  validation.reset()
}

defineExpose({
  formState,
  validation,
  isSubmitting,
  submit,
  reset
})

const emit = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
