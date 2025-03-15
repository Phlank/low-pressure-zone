<template>
  <div class="dashboard-schedules-view">
    <div
      class="dashboard-schedules-view__new-schedules-form"
      v-show="authStore.isInAnySpecifiedRole(Role.Admin, Role.Organizer)">
      <h4>Create New Schedule</h4>
      <ScheduleForm
        ref="createForm"
        :communities="communities.filter((a) => a.isLinkableToSchedule)" />
      <Button
        class="input"
        label="Create"
        @click="handleCreateClick"
        :disabled="isSubmitting" />
    </div>
    <h4>Upcoming Schedules</h4>
    <SchedulesGrid
      :schedules="schedules"
      :communities="communities"
      :performers="performers"
      @update="handleSchedulesUpdate" />
  </div>
</template>

<script lang="ts" setup>
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { onMounted, ref, useTemplateRef, type Ref } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import { useAuthStore } from '@/stores/authStore'
import { Role } from '@/constants/roles'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi.ts'
import performersApi, { type PerformerResponse } from '@/api/resources/performersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const authStore = useAuthStore()
const toast = useToast()
const isSubmitting = ref(false)

onMounted(async () => {
  await Promise.all([loadSchedules(), loadCommunities(), loadPerformers()])
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
  if (createForm.value == undefined) return
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
