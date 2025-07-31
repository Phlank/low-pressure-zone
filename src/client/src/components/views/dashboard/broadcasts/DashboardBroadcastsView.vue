<template>
  <div class="dashboard-broadcasts-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="mine">Mine</Tab>
        <Tab
          v-if="authStore.isInRole(roles.admin)"
          value="all"
          >All
        </Tab>
      </TabList>
      <TabPanels>
        <TabPanel value="mine">
          <BroadcastsGrid />
        </TabPanel>
        <TabPanel value="all">
          <BroadcastsGrid show-streamer-name />
        </TabPanel>
      </TabPanels>
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { onMounted, ref } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import BroadcastsGrid from '@/components/views/dashboard/broadcasts/BroadcastsGrid.vue'
import { Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'

const authStore = useAuthStore()
const broadcastStore = useBroadcastStore()
const tabValue = ref('mine')

onMounted(async () => {
  await broadcastStore.loadIfNotInitialized()
})
</script>
