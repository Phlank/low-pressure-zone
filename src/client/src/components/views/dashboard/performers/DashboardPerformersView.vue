<template>
  <div class="performers-dashboard">
    <Tabs v-model:value="tabValue">
      <TabList>
        <Tab
          value="0"
          :disabled="controlsDisabled">
          Mine
        </Tab>
        <Tab
          value="1"
          :disabled="controlsDisabled">
          All
        </Tab>
        <Tab
          value="2"
          :disabled="controlsDisabled">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="0">
          <LinkedPerformers
            :performers="performers"
            @to-new-performer="tabValue = '2'" />
        </TabPanel>
        <TabPanel value="1">
          <AllPerformers :performers="performers" />
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
import api from '@/api/api'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { computed, onMounted, ref, type Ref } from 'vue'
import AllPerformers from './AllPerformers.vue'
import CreatePerformer from './CreatePerformer.vue'
import LinkedPerformers from './LinkedPerformers.vue'

const tabValue = ref('0')
const isLoaded = ref(false)
const isSubmitting = ref(false)
const isDialogOpen = ref(false)
const controlsDisabled = computed(() => !isLoaded.value || isSubmitting.value || isDialogOpen.value)

const performers: Ref<PerformerResponse[]> = ref([])

onMounted(async () => {
  await loadPerformers()
})

const loadPerformers = async () => {
  const response = await api.performers.get()
  isLoaded.value = true
  if (!response.isSuccess()) {
    return
  }
  performers.value = response.data!
}
</script>
