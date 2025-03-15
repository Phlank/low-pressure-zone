<template>
  <div class="community-form">
    <div>
      <IftaLabel class="input input--medium">
        <InputText
          id="nameInput"
          v-model:model-value="formState.name"
          :disabled="disabled"
          :invalid="!validation.isValid('name')"
          class="input__field"
          @update:model-value="validation.validateIfDirty('name')" />
        <ValidationLabel
          :message="validation.message('name')"
          for="nameInput"
          text="Name" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <InputText
          id="urlInput"
          v-model:model-value="formState.url"
          :disabled="disabled"
          :invalid="!validation.isValid('url')"
          class="input__field"
          @update:model-value="validation.validateIfDirty('url')" />
        <ValidationLabel
          :message="validation.message('url')"
          for="urlInput"
          text="URL" />
      </IftaLabel>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText } from 'primevue'
import { onMounted, reactive } from 'vue'
import ValidationLabel from '../ValidationLabel.vue'
import { communityRequestRules } from '@/validation/requestRules'
import type { CommunityRequest } from '@/api/resources/communitiesApi.ts'

const formState: CommunityRequest = reactive({ name: '', url: '' })
const validation = createFormValidation(formState, communityRequestRules)

const props = defineProps<{ initialState: CommunityRequest; disabled: boolean }>()

onMounted(() => {
  formState.name = props.initialState.name
  formState.url = props.initialState.url
})

const reset = () => {
  formState.name = props.initialState.name
  formState.url = props.initialState.url
  validation.reset()
}

defineExpose({ formState, validation, reset })
</script>
