<template>
  <div class="news-form">
    <FormArea>
      <IftaFormField
        :message="val.message('title')"
        input-id="titleInput"
        label="Title"
        size="xl">
        <InputText
          id="titleInput"
          v-model:model-value="state.title"
          :disabled="isSubmitting"
          :invalid="!val.isValid('title')"
          autofocus />
      </IftaFormField>
      <IftaFormField
        :message="val.message('body')"
        input-id="bodyInput"
        label="Body"
        size="full">
        <Textarea
          id="bodyInput"
          v-model:model-value="state.body"
          :disabled="isSubmitting"
          :invalid="!val.isValid('body')"
          :rows="5"
          auto-resize />
      </IftaFormField>
      <template #actions>
        <slot name="actions"></slot>
      </template>
    </FormArea>
    <MarkdownPreview
      :markdown-content="previewBody"
      :contentTitle="previewTitle" />
  </div>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import type { NewsResponse } from '@/api/resources/newsApi.ts'
import { onMounted, ref, watch } from 'vue'
import { required } from '@/validation/rules/untypedRules.ts'
import { InputText, Textarea } from 'primevue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useDebounceFn } from '@vueuse/core'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import { useNewsStore } from '@/stores/newsStore.ts'
import { useEntityForm } from '@/composables/useEntityForm.ts'
import MarkdownPreview from '@/components/controls/MarkdownPreview.vue'

const news = useNewsStore()

const props = defineProps<{
  newsItem?: NewsResponse
}>()

const { state, val, isSubmitting, submit, reset } = useEntityForm({
  validationRules: {
    title: required(),
    body: required()
  },
  entity: props.newsItem,
  formStateInitializeFn: (newsItem) =>
    ref({
      title: newsItem?.title ?? '',
      body: newsItem?.body ?? ''
    }),
  createPersistentEntityFn: news.create,
  updatePersistentEntityFn: news.update,
  onSubmitted: () => emit('submitted')
})

defineExpose({
  isSubmitting,
  submit,
  reset
})

const emit = defineEmits(['submitted'])

const previewTitle = ref('')
const previewBody = ref('')
const updatePreview = useDebounceFn(async () => {
  previewTitle.value = state.value.title
  previewBody.value = await parseMarkdownAsync(state.value.body)
}, 200)
watch(
  () => [state.value.title, state.value.body],
  () => {
    updatePreview()
  },
  { deep: true, immediate: true }
)

onMounted(() => {
  reset()
})
</script>

<style lang="scss" scoped>
.news-form {
  display: flex;
  flex-direction: column;
  gap: 1.5rem;
}
</style>
