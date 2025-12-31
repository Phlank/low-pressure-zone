<template>
  <div class="news-post">
    <h3 class="news-post__title">{{ title }}</h3>
    <p class="news-post__date">{{ postDate.toLocaleDateString() }}</p>
    <div
      class="news-post__body"
      v-html="bodyHtml" />
  </div>
</template>

<script setup lang="ts">
import { ref, watch } from 'vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'

const props = defineProps<{
  title: string
  body: string
  postDate: Date
}>()

const bodyHtml = ref('')

watch(
  () => props.body,
  async (newVal) => {
    bodyHtml.value = await parseMarkdownAsync(newVal)
  },
  { immediate: true }
)
</script>

<style scoped lang="scss">
.news-post {
  &__title {
    margin-bottom: 0;
  }

  &__date {
    margin-top: 0;
    margin-bottom: 1em;
    color: var(--p-text-muted-color);
  }
}
</style>
