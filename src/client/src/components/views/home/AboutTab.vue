<template>
  <div class="about-section">
    <ExpandableContent>
      <template #top>
        <div v-html="topHtml" />
      </template>
      <template #bottom>
        <div v-html="bottomHtml" />
      </template>
    </ExpandableContent>
  </div>
</template>
<script lang="ts" setup>
import { ref, watch } from 'vue'
import ExpandableContent from '@/components/controls/ExpandableContent.vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import { useAboutSettingsStore } from '@/stores/aboutSettingsStore.ts'

const aboutSettings = useAboutSettingsStore()

watch(
  () => [aboutSettings.topText, aboutSettings.bottomText],
  async () => {
    topHtml.value = await parseMarkdownAsync(aboutSettings.topText)
    bottomHtml.value = await parseMarkdownAsync(aboutSettings.bottomText)
  }
)

const topHtml = ref('')
const bottomHtml = ref('')
</script>
