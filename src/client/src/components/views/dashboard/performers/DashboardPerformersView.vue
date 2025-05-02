<template>
  <div class="performers-dashboard">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="0"> Mine</Tab>
        <Tab value="1"> All</Tab>
        <Tab value="2"> Create</Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="0">
          <div class="performers-dashboard__linked">
            <h4>Your Performers</h4>
            <PerformersGrid
              v-if="performerStore.linkablePerformers.length > 0"
              :performers="performerStore.linkablePerformers" />
            <div v-else>
              <p>You do not currently have any linked performers.</p>
              <Button
                label="Create Performer"
                @click="tabValue = '2'" />
            </div>
          </div>
        </TabPanel>
        <TabPanel value="1">
          <div class="performers-dashboard__all">
            <h4>All Performers</h4>
            <PerformersGrid :performers="performerStore.performers" />
          </div>
        </TabPanel>
        <TabPanel value="2">
          <CreatePerformer />
        </TabPanel>
      </TabPanels>
    </Tabs>
    <Skeleton
      v-show="!isLoaded"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import { Button, Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { onMounted, ref, type Ref } from 'vue'
import CreatePerformer from './CreatePerformer.vue'
import PerformersGrid from './PerformersGrid.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'

const performerStore = usePerformerStore()
const tabValue: Ref<string | number> = ref('0')
const isLoaded = ref(false)

onMounted(async () => {
  if (performerStore.performers.length === 0) {
    await performerStore.loadPerformersAsync()
  }
  isLoaded.value = true
})
</script>
