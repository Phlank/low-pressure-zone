<template>
  <div class="about-section">
    <Skeleton
      v-if="aboutSettings.isLoading"
      style="height: 200px" />
    <ExpandableContent v-else>
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
import { onMounted, ref, watch } from 'vue'
import ExpandableContent from '@/components/controls/ExpandableContent.vue'
import { parseMarkdownAsync } from '@/utils/markdown.ts'
import { useAboutSettingsStore } from '@/stores/aboutSettingsStore.ts'
import { Skeleton } from 'primevue'

const aboutSettings = useAboutSettingsStore()

const topHtml = ref('')
const bottomHtml = ref('')
const updateContent = async () => {
  console.log('Loading')
  topHtml.value = await parseMarkdownAsync(aboutSettings.topText)
  bottomHtml.value = await parseMarkdownAsync(aboutSettings.bottomText)
}

watch(
  () => [aboutSettings.topText, aboutSettings.bottomText, aboutSettings.isLoading],
  async () => {
    await updateContent()
  }
)

onMounted(() => {
  updateContent()
})
</script>
