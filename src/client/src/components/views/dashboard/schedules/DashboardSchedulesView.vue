<template>
  <div class="dashboard-schedules-view">
    <div
      class="dashboard-schedules-view__new-schedules-form"
      v-show="authStore.isInAnySpecifiedRole(Role.Admin, Role.Organizer)">
      <h4>Create New Schedule</h4>
      <ScheduleForm
        ref="createForm"
        :audiences="audiences.filter((a) => a.isLinkableToSchedule)" />
      <Button
        class="input"
        label="Create"
        @click="handleCreateClick"
        :disabled="isSubmitting" />
    </div>
    <h4>Upcoming Schedules</h4>
    <SchedulesGrid
      :schedules="schedules"
      :audiences="audiences"
      :performers="performers"
      @update="handleSchedulesUpdate" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, useToast } from 'primevue'
import { onMounted, ref, useTemplateRef, type Ref } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import { useAuthStore } from '@/stores/authStore'
import { Role } from '@/constants/roles'

const authStore = useAuthStore()
const toast = useToast()
const isSubmitting = ref(false)

onMounted(async () => {
  await Promise.all([loadSchedules(), loadAudiences(), loadPerformers()])
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])
const performers: Ref<PerformerResponse[]> = ref([])
const loadSchedules = async () =>
  (schedules.value = (await api.schedules.get({ after: new Date().toISOString() })).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])
const loadPerformers = async () => (performers.value = (await api.performers.get()).data ?? [])

const createForm = useTemplateRef('createForm')
const handleCreateClick = async () => {
  if (createForm.value == undefined) return
  const isInvalid = createForm.value.validation.validate()
  if (!isInvalid) return

  isSubmitting.value = true
  const response = await api.schedules.post(createForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, createForm.value.validation)) return
  showCreateSuccessToast(toast, 'schedule')
  await loadSchedules()
  createForm.value.reset()
}

const handleSchedulesUpdate = async (scheduleId?: string) => {
  if (scheduleId) {
    const response = await api.schedules.getById(scheduleId)
    if (response.isSuccess()) {
      const index = schedules.value.findIndex((s) => s.id === scheduleId)
      schedules.value.splice(index, 1, response.data!)
    }
  } else {
    const response = await api.schedules.get({ after: new Date().toISOString() })
    if (response.isSuccess()) {
      schedules.value = response.data!
    }
  }
}
</script>
