<template>
  <div class="desktop-inline">
    <IftaLabel class="input input--small">
      <InputText
        id="name"
        v-model:model-value="formState.name"
        :disabled="disabled"
        :invalid="!validation.isValid('name')"
        autofocus
        class="input__field"
        @update:model-value="validation.validateIfDirty('name')" />
      <ValidationLabel
        :message="validation.message('name')"
        for="name"
        >Name
      </ValidationLabel>
    </IftaLabel>
    <IftaLabel class="input input--large">
      <InputText
        id="url"
        v-model:model-value="formState.url"
        :disabled="disabled"
        :invalid="!validation.isValid('url')"
        class="input__field"
        @update:model-value="validation.validateIfDirty('url')" />
      <ValidationLabel
        :message="validation.message('url')"
        for="url"
        >URL
      </ValidationLabel>
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
  initialState?: PerformerRequest
  disabled: boolean
}>()

onMounted(() => {
  reset()
})

const reset = () => {
  formState.name = props.initialState?.name ?? ''
  formState.url = props.initialState?.url ?? ''
  validation.reset()
}

defineExpose({
  formState,
  validation,
  reset
})
</script>
