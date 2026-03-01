<template>
  <div class="markdown-preview">
    <Divider>{{ previewTitle ?? 'Preview' }}</Divider>
    <h2 v-if="contentTitle">{{ contentTitle }}</h2>
    <div v-html="htmlContent"></div>
  </div>
</template>

<script setup lang="ts">
import { Divider } from 'primevue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import { ref, watch } from 'vue'
import { useDebounceFn } from '@vueuse/core'

const props = defineProps<{
  previewTitle?: string
  contentTitle?: string
  markdownContent: string
}>()

const htmlContent = ref('')
const updatePreview = useDebounceFn(async () => {
  htmlContent.value = await parseMarkdownAsync(props.markdownContent)
}, 200)
watch(
  () => [props.markdownContent],
  () => {
    updatePreview()
  },
  { immediate: true }
)
</script>

<style scoped lang="scss">
.markdown-preview {
  display: flex;
  flex-direction: column;
}
</style>
