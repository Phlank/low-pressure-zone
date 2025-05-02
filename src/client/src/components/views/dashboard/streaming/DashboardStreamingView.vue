<template>
  <div class="dashboard-streaming-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="dj-software">DJ software</Tab>
        <Tab value="external">Decks and mixer</Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="dj-software">
          <SoftwareStreamingDirections />
        </TabPanel>
        <TabPanel value="external">
          <ExternalStreamingDirections />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref, type Ref } from 'vue'
import SoftwareStreamingDirections from '@/components/views/dashboard/streaming/SoftwareStreamingDirections.vue'
import ExternalStreamingDirections from '@/components/views/dashboard/streaming/ExternalStreamingDirections.vue'

const tabValue: Ref<string | number> = ref('dj-software')
</script>
