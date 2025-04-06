<template>
  <div class="dashboard-schedules-view">
    <Tabs v-model:value="tabValue">
      <TabList style="width: 100%; overflow-x: scroll">
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
          <SchedulesGrid :schedules="upcomingSchedules" />
        </TabPanel>
        <TabPanel value="past">
          <SchedulesGrid :schedules="pastSchedules" />
        </TabPanel>
        <TabPanel
          v-if="communityStore.organizableCommunities.length > 0"
          value="create">
          <h4>Create New Schedule</h4>
          <ScheduleForm
            ref="createForm"
            :communities="communityStore.organizableCommunities" />
          <Button
            :disabled="isSubmitting"
            class="input"
            label="Create"
            @click="handleCreateClick" />
        </TabPanel>
      </TabPanels>
      <Skeleton
        v-else
        style="height: 300px" />
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Button, Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs, useToast } from 'primevue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { computed, onMounted, ref, type Ref, useTemplateRef } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import schedulesApi from '@/api/resources/schedulesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { compareDateStrings, parseTime } from '@/utils/dateUtils.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'

const scheduleStore = useScheduleStore()
const communityStore = useCommunityStore()
const performerStore = usePerformerStore()
const toast = useToast()
const isSubmitting = ref(false)
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

const upcomingSchedules = computed(() =>
  scheduleStore.schedules.filter((schedule) => parseTime(schedule.endsAt) > new Date().getTime())
)
const pastSchedules = computed(() =>
  scheduleStore.schedules
    .filter((schedule) => parseTime(schedule.endsAt) <= new Date().getTime())
    .sort((a, b) => compareDateStrings(b.startsAt, a.startsAt))
)

const createForm = useTemplateRef('createForm')
const handleCreateClick = async () => {
  if (!createForm.value) return
  const isInvalid = createForm.value.validation.validate()
  if (!isInvalid) return

  isSubmitting.value = true
  const response = await schedulesApi.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return
  showCreateSuccessToast(toast, 'schedule')

  createForm.value.reset()
}
</script>
