<template>
  <div class="desktop-inline">
    <IftaLabel class="input input--small">
      <InputText
        id="name"
        class="input__field"
        v-model:model-value="formState.name"
        @update:model-value="validation.validateIfDirty('name')"
        :disabled="disabled"
        :invalid="!validation.isValid('name')" />
      <ValidationLabel
        for="name"
        :message="validation.message('name')"
        >Name</ValidationLabel
      >
    </IftaLabel>
    <IftaLabel class="input input--large">
      <InputText
        id="url"
        class="input__field"
        v-model:model-value="formState.url"
        @update:model-value="validation.validateIfDirty('url')"
        :disabled="disabled"
        :invalid="!validation.isValid('url')" />
      <ValidationLabel
        for="url"
        :message="validation.message('url')"
        >URL</ValidationLabel
      >
    </IftaLabel>
  </div>
</template>

<script lang="ts" setup>
import { createFormValidation } from '@/validation/types/formValidation'
import { IftaLabel, InputText } from 'primevue'
import { onMounted, reactive } from 'vue'
import ValidationLabel from '../ValidationLabel.vue'
import { performerRequestRules } from '@/validation/requestRules'
import type { PerformerRequest } from '@/api/resources/performersApi.ts'

const formState: PerformerRequest = reactive({ name: '', url: '' })
const validation = createFormValidation(formState, performerRequestRules)

const props = defineProps<{
  initialState: PerformerRequest
  disabled: boolean
}>()

onMounted(() => {
  formState.name = props.initialState.name
  formState.url = props.initialState.url
})

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
