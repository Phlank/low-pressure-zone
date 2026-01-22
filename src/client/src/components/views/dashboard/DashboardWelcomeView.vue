<template>
  <div class="dashboard-welcome-view">
    <Tabs :value="currentTab">
      <TabList>
        <Tab
          v-for="tab in welcomeSettings.tabs"
          :key="tab.title"
          :value="tab.title">
          {{ tab.title }}
        </Tab>
      </TabList>
      <TabPanels>
        <TabPanel
          v-for="tab in welcomeSettings.tabs"
          :key="tab.title"
          :value="tab.title">
          <MarkdownContent :content="tab.body" />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { useWelcomeSettingsStore } from '@/stores/settings/welcomeSettingsStore.ts'
import { ref, watch } from 'vue'
import MarkdownContent from '@/components/controls/MarkdownContent.vue'
import { Tab, TabPanel, TabPanels, Tabs, TabList } from 'primevue'

const welcomeSettings = useWelcomeSettingsStore()
const currentTab = ref(welcomeSettings.tabs[0]?.title ?? '')
watch(
  () => welcomeSettings.tabs,
  (newTabs) => {
    if (!newTabs.some((tab) => tab.title === currentTab.value)) {
      currentTab.value = newTabs[0]?.title ?? ''
    }
  }
)
</script>
