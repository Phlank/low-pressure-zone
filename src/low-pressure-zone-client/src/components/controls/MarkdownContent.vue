<template>
  <div
    class="markdown-content"
    v-html="htmlContent"></div>
</template>

<script lang="ts" setup>
import { ref, watch } from 'vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'

const props = defineProps<{
  content: string
}>()

const htmlContent = ref('')
watch(
  () => props.content,
  async (newContent) => {
    htmlContent.value = await parseMarkdownAsync(newContent)
  },
  { immediate: true }
)
</script>
