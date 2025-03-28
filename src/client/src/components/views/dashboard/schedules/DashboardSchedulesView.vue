<template>
  <div class="dashboard-schedules-view">
    <div
      v-show="
        authStore.isInAnySpecifiedRole(Role.Admin) ||
        communityStore.communities.some((community) => community.isOrganizable)
      "
      class="dashboard-schedules-view__new-schedules-form">
      <h4>Create New Schedule</h4>
      <ScheduleForm
        ref="createForm"
        :communities="communities.filter((a) => a.isOrganizable)" />
      <Button
        :disabled="isSubmitting"
        class="input"
        label="Create"
        @click="handleCreateClick" />
    </div>
    <h4>Upcoming Schedules</h4>
    <SchedulesGrid
      :communities="communities"
      :performers="performers"
      :schedules="schedules"
      @update="handleSchedulesUpdate" />
  </div>
</template>

<script lang="ts" setup>
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { onMounted, ref, type Ref, useTemplateRef } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import { useAuthStore } from '@/stores/authStore'
import { Role } from '@/constants/role.ts'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import performersApi, { type PerformerResponse } from '@/api/resources/performersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'

const authStore = useAuthStore()
const communityStore = useCommunityStore()
const toast = useToast()
const isSubmitting = ref(false)

onMounted(async () => {
  const promises: Promise<unknown>[] = [loadSchedules(), loadCommunities(), loadPerformers()]
  if (communityStore.communities.length === 0) {
    promises.push(communityStore.loadCommunitiesAsync())
  }
  await Promise.all(promises)
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const communities: Ref<CommunityResponse[]> = ref([])
const performers: Ref<PerformerResponse[]> = ref([])
const loadSchedules = async () =>
  (schedules.value = (await schedulesApi.get({ after: new Date().toISOString() })).data ?? [])
const loadCommunities = async () => (communities.value = (await communitiesApi.get()).data ?? [])
const loadPerformers = async () => (performers.value = (await performersApi.get()).data ?? [])

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
  await loadSchedules()
  createForm.value.reset()
}

const handleSchedulesUpdate = async (scheduleId?: string) => {
  if (scheduleId) {
    const response = await schedulesApi.getById(scheduleId)
    if (response.isSuccess()) {
      const index = schedules.value.findIndex((s) => s.id === scheduleId)
      schedules.value.splice(index, 1, response.data!)
    }
  } else {
    const response = await schedulesApi.get({ after: new Date().toISOString() })
    if (response.isSuccess()) {
      schedules.value = response.data!
    }
  }
}
</script>
