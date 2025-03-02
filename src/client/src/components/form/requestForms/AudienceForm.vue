<template>
  <div class="audience-form">
    <div class="desktop-inline">
      <IftaLabel class="input input--small">
        <InputText
          id="nameInput"
          class="input__field"
          :value="formState.name"
          @update:model-value="handleNameUpdate"
          :disabled="disabled"
          :invalid="!validation.isValid('name')" />
        <ValidationLabel
          for="nameInput"
          :message="validation.message('name')"
          text="Name" />
      </IftaLabel>
      <IftaLabel class="input input--large">
        <InputText
          class="input__field"
          id="urlInput"
          :value="formState.url"
          @update:model-value="handleUrlUpdate"
          :disabled="disabled"
          :invalid="!validation.isValid('url')" />
        <ValidationLabel
          for="urlInput"
          :message="validation.message('url')"
          text="URL" />
      </IftaLabel>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText } from 'primevue'
import { onMounted, reactive } from 'vue'
import { createUpdateHandler } from '../formInputHandling'
import ValidationLabel from '../ValidationLabel.vue'
import type { AudienceRequest } from '@/api/audiences/audienceRequest'
import { audienceRequestRules } from '@/validation/requestRules'

const formState: AudienceRequest = reactive({ name: '', url: '' })
const validation = createFormValidation(formState, audienceRequestRules)

const props = defineProps<{ initialState: AudienceRequest; disabled: boolean }>()

onMounted(() => {
  formState.name = props.initialState.name
  formState.url = props.initialState.url
})

const handleNameUpdate = createUpdateHandler(formState, validation, 'name', '')
const handleUrlUpdate = createUpdateHandler(formState, validation, 'url', '')

const reset = () => {
  formState.name = props.initialState.name
  formState.url = props.initialState.url
  validation.reset()
}

defineExpose({ formState, validation, reset })
</script>
