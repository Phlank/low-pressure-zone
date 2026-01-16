<template>
  <div class="about-section">
    <ExpandableContent v-if="!aboutSettings.isLoading">
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
import { useAboutSettingsStore } from '@/stores/settings/aboutSettingsStore.ts'

const aboutSettings = useAboutSettingsStore()

const topHtml = ref('')
const bottomHtml = ref('')

const updateContent = async () => {
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
