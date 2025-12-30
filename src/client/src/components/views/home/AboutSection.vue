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
import { useToast } from 'primevue'
import { ref, watch } from 'vue'
import ExpandableContent from '@/components/controls/ExpandableContent.vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import useAboutSettings from '@/composables/settings/useAboutSettings.ts'

const toast = useToast()
const aboutSettings = useAboutSettings(toast)

watch(
  () => [aboutSettings.topText.value, aboutSettings.bottomText.value],
  async () => {
    topHtml.value = await parseMarkdownAsync(aboutSettings.topText.value)
    bottomHtml.value = await parseMarkdownAsync(aboutSettings.bottomText.value)
  }
)

const topHtml = ref('')
const bottomHtml = ref('')
</script>
