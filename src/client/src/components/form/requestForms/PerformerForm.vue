<template>
  <div class="performer-form">
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
        size="l">
        <InputText
          id="urlInput"
          v-model:model-value="formState.url"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('url')"
          @update:model-value="validation.validateIfDirty('url')" />
      </IftaFormField>
      <template #actions>
        <Button
          v-if="!props.hideActions"
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
import { performerRequestRules } from '@/validation/requestRules'
import { type PerformerRequest, type PerformerResponse } from '@/api/resources/performersApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import type { Result } from '@/types/result.ts'

const performers = usePerformerStore()
const formState: Ref<PerformerRequest> = ref({ name: '', url: '' })
const validation = createFormValidation(formState, performerRequestRules)

const props = withDefaults(
  defineProps<{
    performer?: PerformerResponse
    hideActions?: boolean
  }>(),
  {
    hideActions: false
  }
)

const isSubmitting = ref(false)
const submit = async () => {
  let result: Result
  isSubmitting.value = true
  if (props.performer) {
    result = await performers.update(props.performer.id, formState, validation)
  } else {
    result = await performers.create(formState, validation)
  }
  isSubmitting.value = false
  if (!result.isSuccess) return
  reset()
  emit('submitted')
}

const reset = () => {
  formState.value.name = props.performer?.name ?? ''
  formState.value.url = props.performer?.url ?? ''
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
