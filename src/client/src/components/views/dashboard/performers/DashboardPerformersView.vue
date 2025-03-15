<template>
  <div class="performers-dashboard">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab value="0"> Mine </Tab>
        <Tab value="1"> All </Tab>
        <Tab value="2"> Create </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="0">
          <div class="performers-dashboard__linked">
            <h4>Your Performers</h4>
            <PerformersGrid
              v-if="linkedPerformers.length > 0"
              :performers="linkedPerformers" />
            <div v-else>
              <p>You do not currently have any linked performers.</p>
              <Button
                @click="tabValue = '2'"
                label="Create Performer" />
            </div>
          </div>
        </TabPanel>
        <TabPanel value="1">
          <div class="performers-dashboard__all">
            <h4>All Performers</h4>
            <PerformersGrid :performers="performers" />
          </div>
        </TabPanel>
        <TabPanel value="2">
          <CreatePerformer @created-performer="loadPerformers()" />
        </TabPanel>
      </TabPanels>
    </Tabs>
    <Skeleton
      v-show="!isLoaded"
      style="height: 300px" />
  </div>
</template>

<script lang="ts" setup>
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs, Button } from 'primevue'
import { computed, onMounted, ref, type Ref } from 'vue'
import CreatePerformer from './CreatePerformer.vue'
import PerformersGrid from './PerformersGrid.vue'
import performersApi, { type PerformerResponse } from '@/api/resources/performersApi.ts'

const tabValue: Ref<string | number> = ref('0')
const isLoaded = ref(false)

const performers: Ref<PerformerResponse[]> = ref([])
const linkedPerformers = computed(() => performers.value.filter((p) => p.isLinkableToTimeslot))

onMounted(async () => {
  await loadPerformers()
})

const loadPerformers = async () => {
  const response = await performersApi.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
}
</script>
