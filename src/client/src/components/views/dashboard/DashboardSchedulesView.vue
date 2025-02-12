<template>
  <div class="desktop-inline">
    <ScheduleForm ref="createForm" :audiences="audiences" :disabled="false" />
    <Button class="input" label="Create" @click="handleCreateClick" />
  </div>
  <DataTable v-if="isLoaded" data-key="id" :value="schedules" :expanded-rows="expandedRows">
    <Column expander style="width: 5rem" />
    <Column field="audience.name" header="Audience" />
    <Column header="Date">
      <template #body="{ data }">
        {{ new Date(Date.parse(data.start)).toLocaleDateString() }}
      </template>
    </Column>
    <Column header="Start">
      <template #body="{ data }">
        {{ new Date(Date.parse(data.start)).toLocaleTimeString() }}
      </template>
    </Column>
    <Column header="End">
      <template #body="{ data }">
        {{ new Date(Date.parse(data.end)).toLocaleTimeString() }}
      </template>
    </Column>
    <Column style="text-align: right">
      <template #body="{ data }">
        <Button
          v-if="canEdit(data)"
          icon="pi pi-pencil"
          @click="handleEditScheduleActionClick(data)"
          rounded
          outlined
        />
        <Button
          v-if="data.timeslots.length === 0"
          class="action"
          icon="pi pi-trash"
          severity="danger"
          @click="handleDeleteScheduleActionClick(data)"
          rounded
          outlined
        />
      </template>
    </Column>
    <template #expansion="rowProps">
      <DataTable data-key="id" :value="rowProps.data.timeslots">
        <Column field="start" header="Start">
          <template #body="timeslotProps">
            {{ timeslotProps.data.start.toLocaleDateString() }}
          </template>
        </Column>
        <Column field="performer.name" header="Performer" />
      </DataTable>
    </template>
  </DataTable>
  <FormDialog
    header="Edit Schedule"
    :is-submitting="isSubmitting"
    :visible="showEditScheduleDialog"
    @close="showEditScheduleDialog = false"
    @save="handleEditScheduleSave"
  >
    <ScheduleForm
      ref="scheduleEditForm"
      :audiences="audiences"
      :disabled="isSubmitting"
      :initial-state="editScheduleFormInitialState"
    />
  </FormDialog>
  <DeleteDialog
    entity-type="schedule"
    header="Delete Schedule"
    :is-submitting="isSubmitting"
    :visible="showDeleteScheduleDialog"
    @close="showDeleteScheduleDialog = false"
    @delete="handleDelete"
  />
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { setToNextHour } from '@/utils/dateUtils'
import { showCreateSuccessToast, showEditSuccessToast } from '@/utils/toastUtils'
import { Button, DataTable, useToast, Column } from 'primevue'
import { onMounted, reactive, ref, useTemplateRef, type Ref } from 'vue'

const toast = useToast()
const isSubmitting = ref(false)
const isLoaded = ref(false)

onMounted(async () => {
  await Promise.all([loadSchedules(), loadAudiences()])
  isLoaded.value = true
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])
const loadSchedules = async () => (schedules.value = (await api.schedules.get()).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])

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

const scheduleEditForm = useTemplateRef('scheduleEditForm')
const showEditScheduleDialog = ref(false)
let editingId = ''
const editScheduleFormInitialState = reactive({
  audienceId: '',
  start: '',
  end: '',
  startTime: new Date(),
  endTime: new Date()
})
const editLimit = new Date() // Schedule times prior to this can't be altered
setToNextHour(editLimit)
editLimit.setDate(editLimit.getDate() - 1)
const canEdit = (schedule: ScheduleResponse) => {
  return Date.parse(schedule.end) > editLimit.getTime()
}
const handleEditScheduleActionClick = (schedule: ScheduleResponse) => {
  editingId = schedule.id
  editScheduleFormInitialState.audienceId = schedule.audience.id
  editScheduleFormInitialState.start = schedule.start
  editScheduleFormInitialState.end = schedule.end
  editScheduleFormInitialState.startTime = new Date(Date.parse(schedule.start))
  editScheduleFormInitialState.endTime = new Date(Date.parse(schedule.end))
  showEditScheduleDialog.value = true
}
const handleEditScheduleSave = async () => {
  if (scheduleEditForm.value == undefined) return
  const isValid = scheduleEditForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.schedules.put(editingId, scheduleEditForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, scheduleEditForm.value.validation)) return
  showEditSuccessToast(toast, 'schedule')
  showEditScheduleDialog.value = false

  const newScheduleResponse = await api.schedules.getById(editingId)
  if (tryHandleUnsuccessfulResponse(newScheduleResponse, toast)) return
  const indexToSplice = schedules.value.findIndex((s) => s.id == editingId)
  schedules.value.splice(indexToSplice, 1, newScheduleResponse.data!)
  schedules.value.sort((a, b) => {
    if (a.start > b.start) return -1
    if (a.start < b.start) return 1
    return 0
  })
}

let deletingId = ''
const showDeleteScheduleDialog = ref(false)
const handleDeleteScheduleActionClick = (schedule: ScheduleResponse) => {
  deletingId = schedule.id
  showDeleteScheduleDialog.value = true
}
const handleDelete = async () => {
  showDeleteScheduleDialog.value = false
  const response = await api.schedules.delete(deletingId)
  tryHandleUnsuccessfulResponse(response, toast)
  schedules.value.splice(
    schedules.value.findIndex((s) => s.id == deletingId),
    1
  )
}

// TIMESLOTS
const expandedRows = ref({})
</script>
