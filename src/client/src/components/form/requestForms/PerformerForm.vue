<template>
  <div class="performer-form">
    <FormArea>
      <IftaFormField
        :message="val.message('name')"
        input-id="nameInput"
        label="Name"
        size="m">
        <InputText
          id="nameInput"
          v-model:model-value="state.name"
          :disabled="isSubmitting"
          :invalid="!val.isValid('name')"
          autofocus
          @update:model-value="val.validateIfDirty('name')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('url')"
        input-id="urlInput"
        label="URL"
        size="l">
        <InputText
          id="urlInput"
          v-model:model-value="state.url"
          :disabled="isSubmitting"
          :invalid="!val.isValid('url')"
          @update:model-value="val.validateIfDirty('url')" />
      </IftaFormField>
      <template #actions>
        <slot name="actions"></slot>
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { InputText } from 'primevue'
import { onMounted, ref } from 'vue'
import { performerRequestRules } from '@/validation/requestRules'
import { type PerformerResponse } from '@/api/resources/performersApi.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormArea from '@/components/form/FormArea.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { useEntityForm } from '@/composables/useEntityForm.ts'

const performers = usePerformerStore()

const props = defineProps<{
  performer?: PerformerResponse
}>()

const { state, val, isSubmitting, submit, reset } = useEntityForm({
  validationRules: performerRequestRules,
  entity: props.performer,
  formStateInitializeFn: (performer) =>
    ref({
      name: performer?.name ?? '',
      url: performer?.url ?? ''
    }),
  createPersistentEntityFn: performers.create,
  updatePersistentEntityFn: performers.update,
  onSubmitted: () => emit('submitted')
})

defineExpose({
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
