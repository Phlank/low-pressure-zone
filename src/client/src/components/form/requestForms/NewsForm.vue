<template>
  <div class="news-form">
    <FormArea>
      <IftaFormField
        :message="validation.message('title')"
        input-id="titleInput"
        label="Title"
        size="xl">
        <InputText
          id="titleInput"
          v-model:model-value="formState.title"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('title')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('body')"
        input-id="bodyInput"
        label="Body"
        size="full">
        <Textarea
          id="bodyInput"
          v-model:model-value="formState.body"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('body')"
          :rows="5"
          auto-resize />
      </IftaFormField>
      <template #actions>
        <Button
          v-if="!hideSubmit"
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Submit"
          @click="submit" />
      </template>
    </FormArea>
    <div class="news-form__preview">
      <Divider>Preview</Divider>
      <h2>{{ previewTitle }}</h2>
      <div v-html="previewBody"></div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import type { NewsRequest, NewsResponse } from '@/api/resources/newsApi.ts'
import { onMounted, ref, type Ref, watch } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { required } from '@/validation/rules/untypedRules.ts'
import { Button, Divider, InputText, Textarea } from 'primevue'
import type { Result } from '@/types/result.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useDebounceFn } from '@vueuse/core'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import { useNewsStore } from '@/stores/newsStore.ts'

const news = useNewsStore()

const props = defineProps<{
  initialData?: NewsResponse
  hideSubmit?: boolean
}>()

const formState: Ref<NewsRequest> = ref({
  title: '',
  body: ''
})
const validation = createFormValidation(formState, {
  title: required(),
  body: required()
})

const isSubmitting = ref(false)
const submit = async () => {
  isSubmitting.value = true
  let result: Result
  if (props.initialData?.id) result = await news.update(props.initialData.id, formState, validation)
  else result = await news.create(formState, validation)
  isSubmitting.value = false
  if (result.isSuccess) {
    reset()
    emit('submitted')
  }
}
const reset = () => {
  formState.value.title = props.initialData?.title ?? ''
  formState.value.body = props.initialData?.body ?? ''
}

onMounted(() => reset())

defineExpose({
  formState,
  validation,
  isSubmitting,
  submit,
  reset
})

const emit = defineEmits(['submitted'])

const previewTitle = ref('')
const previewBody = ref('')
const updatePreview = useDebounceFn(async () => {
  previewTitle.value = formState.value.title
  previewBody.value = await parseMarkdownAsync(formState.value.body)
}, 200)
watch(
  () => [formState.value.title, formState.value.body],
  () => {
    updatePreview()
  },
  { deep: true, immediate: true }
)

onMounted(() => {

})
</script>

<style lang="scss" scoped>
.news-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;

  &__preview {
    display: flex;
    flex-direction: column;
  }
}
</style>
