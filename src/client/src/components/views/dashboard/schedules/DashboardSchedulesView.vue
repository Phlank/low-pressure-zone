<template>
  <div class="dashboard-schedules-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="upcoming">Upcoming</Tab>
        <Tab value="past">Past</Tab>
        <Tab
          v-if="communityStore.organizableCommunities.length > 0"
          value="create">
          Create
        </Tab>
      </TabList>
      <TabPanels v-if="isLoaded">
        <TabPanel value="upcoming">
          <SchedulesGrid :schedules="scheduleStore.upcomingSchedules" />
        </TabPanel>
        <TabPanel value="past">
          <SchedulesGrid :schedules="scheduleStore.pastSchedules" />
        </TabPanel>
        <TabPanel
          v-if="communityStore.organizableCommunities.length > 0"
          value="create">
          <h4>Create New Schedule</h4>
          <ScheduleForm
            ref="createForm"
            :communities="communityStore.organizableCommunities" />
        </TabPanel>
      </TabPanels>
      <Skeleton
        v-else
        style="height: 300px" />
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { onMounted, ref, type Ref } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'

const scheduleStore = useScheduleStore()
const communityStore = useCommunityStore()
const performerStore = usePerformerStore()
const isLoaded = ref(false)
const tabValue: Ref<string | number> = ref('upcoming')

onMounted(async () => {
  await load()
  isLoaded.value = true
})

const load = async () => {
  const loadingPromises: Promise<unknown>[] = []
  if (communityStore.communities.length === 0)
    loadingPromises.push(communityStore.loadCommunitiesAsync())
  if (scheduleStore.schedules.length === 0)
    loadingPromises.push(scheduleStore.loadDefaultSchedulesAsync())
  if (performerStore.performers.length === 0)
    loadingPromises.push(performerStore.loadPerformersAsync())
  await Promise.all(loadingPromises)
}
</script>
