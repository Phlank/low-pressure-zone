<template>
  <div class="about-tab">
    <p class="about-tab__form-help">Markdown formatting is rendered as HTML.</p>
    <div class="about-tab__sections">
      <AboutSettingsForm ref="formRef" />
      <div class="about-tab__preview-section">
        <Divider>Preview</Divider>
        <ExpandableContent class="about-tab__preview">
          <template #top>
            <div v-html="topPreviewText" />
          </template>
          <template #bottom>
            <div v-html="bottomPreviewText" />
          </template>
        </ExpandableContent>
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Divider } from 'primevue'
import { computed, inject, type Ref, ref, useTemplateRef, watch } from 'vue'
import { useDebounceFn } from '@vueuse/core'
import ExpandableContent from '@/components/controls/ExpandableContent.vue'
import AboutSettingsForm from '@/components/form/requestForms/AboutSettingsForm.vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'

const isMobile: Ref<boolean> | undefined = inject<Ref<boolean>>('isMobile')
const formRef = useTemplateRef('formRef')

const topPreviewText = ref('')
const bottomPreviewText = ref('')
const updatePreview = useDebounceFn(async (): Promise<void> => {
  topPreviewText.value = await parseMarkdownAsync(formRef.value?.formState.topText ?? '')
  bottomPreviewText.value = await parseMarkdownAsync(formRef.value?.formState.bottomText ?? '')
}, 200)

watch(
  () => formRef.value?.formState,
  async () => {
    await updatePreview()
  },
  { deep: true }
)

const sectionDisplayStyle = computed(() => (isMobile?.value ? 'block' : 'flex'))
const sectionWidthStyle = computed(() => (isMobile?.value ? '100%' : '50%'))
</script>

<style lang="scss" scoped>
.about-tab {
  display: flex;
  flex-direction: column;
  align-content: center;

  &__form-help {
    text-align: center;
  }

  &__sections {
    display: v-bind(sectionDisplayStyle);
    gap: 1rem;

    :deep(div.about-settings-form) {
      width: v-bind(sectionWidthStyle);
    }

    .about-tab__preview-section {
      width: v-bind(sectionWidthStyle);
    }
  }
}
</style>
