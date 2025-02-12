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
        <Button v-if="canEdit(data)" icon="pi pi-pencil" rounded outlined />
        <Button
          v-if="data.timeslots.length === 0"
          class="action"
          icon="pi pi-trash"
          severity="danger"
          @click="handleDeleteActionClick(data)"
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
  <DeleteDialog
    entity-type="schedule"
    header="Delete Schedule"
    :is-submitting="isSubmitting"
    :visible="showDeleteDialog"
    @close="showDeleteDialog = false"
    @delete="handleDelete"
  />
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { setToNextHour } from '@/utils/dateUtils'
import { showCreateSuccessToast } from '@/utils/toastUtils'
import { Button, DataTable, useToast, Column } from 'primevue'
import { onMounted, ref, useTemplateRef, type Ref } from 'vue'

const toast = useToast()
const isSubmitting = ref(false)
const isLoaded = ref(false)
const expandedRows = ref({})

onMounted(async () => {
  await Promise.all([loadSchedules(), loadAudiences()])
  isLoaded.value = true
})

const schedules: Ref<ScheduleResponse[]> = ref([])
const audiences: Ref<AudienceResponse[]> = ref([])
const loadSchedules = async () => (schedules.value = (await api.schedules.get()).data ?? [])
const loadAudiences = async () => (audiences.value = (await api.audiences.get()).data ?? [])

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

const editLimit = new Date() // Schedule times prior to this can't be altered
setToNextHour(editLimit)
editLimit.setDate(editLimit.getDate() - 1)
const canEdit = (schedule: ScheduleResponse) => {
  return Date.parse(schedule.end) > editLimit.getTime()
}

let deletingId = ''
const showDeleteDialog = ref(false)
const handleDeleteActionClick = (schedule: ScheduleResponse) => {
  deletingId = schedule.id
  showDeleteDialog.value = true
}
const handleDelete = async () => {
  showDeleteDialog.value = false
  const response = await api.schedules.delete(deletingId)
  tryHandleUnsuccessfulResponse(response, toast)
  schedules.value.splice(
    schedules.value.findIndex((s) => s.id == deletingId),
    1
  )
}
</script>
