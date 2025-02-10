<template>
  <div class="performer-form desktop-inline-form">
    <IftaLabel class="input">
      <InputText
        id="name"
        :value="formState.name"
        @update:model-value="handleNameUpdate"
        :disabled="isSubmitting"
        :invalid="!validation.isValid('name')"
      />
      <ValidationLabel for="name" :message="validation.message('name')">Name</ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input">
      <InputText
        class="input__field--large"
        id="url"
        :value="formState.url"
        @update:model-value="handleUrlUpdate"
        :disabled="isSubmitting"
        :invalid="!validation.isValid('url')"
      />
      <ValidationLabel for="url" :message="validation.message('url')">URL</ValidationLabel>
    </IftaLabel>
  </div>
</template>

<script lang="ts" setup>
import { nameValidator, urlValidator } from '@/validation/rules/composed/performerValidators'
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText } from 'primevue'
import { onMounted, reactive } from 'vue'
import { createUpdateHandler } from '../formInputHandling'
import ValidationLabel from '../ValidationLabel.vue'
import type { PerformerRequest } from '@/api/performers/performerRequest'

const formState: PerformerRequest = reactive({ name: '', url: '' })
const validation = createFormValidation(formState, {
  name: nameValidator,
  url: urlValidator
})

const props = defineProps<{
  initialState: PerformerRequest
  isSubmitting: boolean
}>()

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

defineExpose({
  formState,
  validation,
  reset
})
</script>
