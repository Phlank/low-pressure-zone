<template>
  <div class="dashboard-streaming-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="dj-software">DJ software</Tab>
        <Tab value="external">Decks and mixer</Tab>
        <Tab value="info">Streaming info</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="dj-software">
          <SoftwareStreamingDirections />
        </TabPanel>
        <TabPanel value="external">
          <ExternalStreamingDirections />
        </TabPanel>
        <TabPanel value="info">
          <StreamInformation />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs, useToast } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import SoftwareStreamingDirections from '@/components/views/dashboard/streaming/SoftwareStreamingDirections.vue'
import ExternalStreamingDirections from '@/components/views/dashboard/streaming/ExternalStreamingDirections.vue'
import StreamInformation from '@/components/views/dashboard/streaming/StreamInformation.vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { showErrorToast } from '@/utils/toastUtils.ts'

const toast = useToast()
const connectionInfoStore = useConnectionInfoStore()

const tabValue: Ref<string | number> = ref('dj-software')

onMounted(async () => {
  const loadResult = await connectionInfoStore.loadIfNotInitialized()
  if (!loadResult.isSuccess) {
    showErrorToast(toast, loadResult.error!)
  }
})
</script>
