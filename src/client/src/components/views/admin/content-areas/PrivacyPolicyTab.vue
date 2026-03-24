<template>
  <div class="privacy-policy-tab">
    <p class="privacy-policy-tab__form-help">Markdown formatting is rendered as HTML.</p>
    <div class="privacy-policy-tab__sections">
      <PrivacyPolicySettingsForm ref="formRef" />
      <div class="privacy-policy-tab__preview-section">
        <MarkdownPreview :markdown-content="previewText" />
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, inject, type Ref, ref, useTemplateRef, watch } from 'vue'
import MarkdownPreview from '@/components/controls/MarkdownPreview.vue'
import { useDebounceFn } from '@vueuse/core'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import PrivacyPolicySettingsForm from "@/components/form/requestForms/PrivacyPolicySettingsForm.vue";

const formRef = useTemplateRef('formRef')
const isMobile: Ref<boolean> | undefined = inject('isMobile')

const previewText = ref('')

watch(
  () => formRef.value?.form,
  async () => {
    await updatePreview()
  },
  { deep: true }
)

const updatePreview = useDebounceFn(async (): Promise<void> => {
  previewText.value = await parseMarkdownAsync(formRef.value?.form.privacyPolicy ?? '')
})

const sectionDisplayStyle = computed(() => (isMobile?.value ? 'block' : 'flex'))
const sectionWidthStyle = computed(() => (isMobile?.value ? '100%' : '50%'))
</script>

<style lang="scss" scoped>
.privacy-policy-tab {
  display: flex;
  flex-direction: column;
  align-content: center;

  &__form-help {
    text-align: center;
  }

  &__sections {
    display: v-bind(sectionDisplayStyle);
    gap: 1rem;
  }

  :deep(div.privacy-policy-settings-form) {
    width: v-bind(sectionWidthStyle);
  }

  :deep(div.privacy-policy-tab__preview-section) {
    width: v-bind(sectionWidthStyle);
  }
}
</style>
