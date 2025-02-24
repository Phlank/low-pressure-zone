<template>
  <div class="dashboard-schedules-view">
    <h4>Create New Schedule</h4>
    <ScheduleForm
      ref="createForm"
      :audiences="audiences" />
    <Button
      class="input"
      label="Create"
      @click="handleCreateClick"
      :disabled="controlsDisabled" />
    <h4>Upcoming Schedules</h4>
    <SchedulesGrid
      :schedules="schedules"
      :audiences="audiences"
      :performers="performers" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { formatTimeslot, setToNextHour, parseDate } from '@/utils/dateUtils'
import { showCreateSuccessToast, showEditSuccessToast } from '@/utils/toastUtils'
import { Button, Column, DataTable, useToast } from 'primevue'
import { computed, onMounted, reactive, ref, useTemplateRef, type Ref } from 'vue'
import DashboardSchedulesTimeslotsTable from './DashboardSchedulesTimeslotsTable.vue'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import SchedulesGrid from './SchedulesGrid.vue'

const toast = useToast()
const isSubmitting = ref(false)
const isLoaded = ref(false)
const isEditingTimeslot = ref(false)
const controlsDisabled = computed(
  () => !isLoaded.value || isSubmitting.value || isEditingTimeslot.value
)

onMounted(async () => {
  await Promise.all([loadSchedules(), loadAudiences(), loadPerformers()])
  isLoaded.value = true
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])
const performers: Ref<PerformerResponse[]> = ref([])
const loadSchedules = async () =>
  (schedules.value = (await api.schedules.get({ after: new Date().toISOString() })).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])
const loadPerformers = async () => (performers.value = (await api.performers.get()).data ?? [])

// SCHEDULES
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
</script>
