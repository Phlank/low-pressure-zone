<template>
  <div class="schedules-grid">
    <div>
      <DataTable
        class="schedules-grid__table"
        data-key="id"
        :value="schedules"
        :expanded-rows="expandedRows"
        sort-field="start"
        :sort-order="1">
        <template>
          <Column
            expander
            style="width: fit-content" />
          <Column
            v-if="!isMobile"
            field="audience.name"
            header="Audience" />
          <Column
            v-if="!isMobile"
            field="start"
            header="Date"
            sortable>
            <template #body="{ data }">
              {{ parseDate(data.start).toLocaleDateString() }}
            </template>
          </Column>
          <Column
            v-if="!isMobile"
            field="end"
            header="Time">
            <template #body="{ data }">
              {{ formatTimeslot(parseDate(data.start)) }} -
              {{ formatTimeslot(parseDate(data.end)) }}
            </template>
          </Column>
          <Column v-if="isMobile">
            <template #body="{ data }">
              <div>{{ parseDate(data.start).toLocaleDateString() }}</div>
              <div class="text-s ellipsis">{{ data.audience.name }}</div>
            </template>
          </Column>
          <Column class="grid-action-col grid-action-col--2">
            <template #body="{ data }">
              <GridActions
                :show-edit="canEdit(data)"
                :show-delete="data.timeslots.length === 0"
                @edit="handleEditScheduleActionClick(data)"
                @delete="handleDeleteScheduleActionClick(data)" />
            </template>
          </Column>
        </template>
        <template #expansion="rowProps">
          <TimeslotsGrid
            :schedule="rowProps.data"
            :performers="performers"
            :disabled="false"
            @update="emit('update', rowProps.data.id)" />
        </template>
      </DataTable>
    </div>
    <FormDialog
      header="Edit Schedule"
      :visible="showEditScheduleDialog"
      :is-submitting="false"
      @close="showEditScheduleDialog = false"
      @save="handleEditScheduleSave">
      <ScheduleForm
        ref="scheduleEditForm"
        :audiences="audiences"
        :initial-state="editScheduleFormInitialState" />
    </FormDialog>
    <DeleteDialog
      entity-type="schedule"
      header="Delete Schedule"
      :is-submitting="false"
      :visible="showDeleteScheduleDialog"
      @close="showDeleteScheduleDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { AudienceResponse } from '@/api/audiences/audienceResponse'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import GridActions from '@/components/data/GridActions.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { formatTimeslot, parseDate, setToNextHour } from '@/utils/dateUtils'
import { showEditSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { inject, reactive, ref, useTemplateRef, type Ref } from 'vue'
import TimeslotsGrid from './TimeslotsGrid.vue'

const expandedRows = ref({})
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const isSubmitting = ref(false)
const toast = useToast()

const props = defineProps<{
  schedules: ScheduleResponse[]
  performers: PerformerResponse[]
  audiences: AudienceResponse[]
}>()

const editLimit = new Date() // Schedule times prior to this can't be altered
setToNextHour(editLimit)
editLimit.setDate(editLimit.getDate() - 1)
const canEdit = (schedule: ScheduleResponse) => {
  return Date.parse(schedule.end) > editLimit.getTime()
}

const scheduleEditForm = useTemplateRef('scheduleEditForm')
const showEditScheduleDialog = ref(false)
let editingId = ''
const editScheduleFormInitialState = reactive({
  audienceId: '',
  start: '',
  end: '',
  description: '',
  startTime: new Date(),
  endTime: new Date()
})

const handleEditScheduleActionClick = (schedule: ScheduleResponse) => {
  editingId = schedule.id
  editScheduleFormInitialState.audienceId = schedule.audience.id
  editScheduleFormInitialState.start = schedule.start
  editScheduleFormInitialState.end = schedule.end
  editScheduleFormInitialState.description = schedule.description
  editScheduleFormInitialState.startTime = parseDate(schedule.start)
  editScheduleFormInitialState.endTime = parseDate(schedule.end)
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

  emit('update', editingId)
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

  emit('update')
}

const emit = defineEmits<{ update: [scheduleId?: string] }>()
</script>

<style lang="scss">
.schedules-grid {
  &__table {
    .p-datatable-row-expansion > td {
      padding: 0;
    }
  }
}
</style>
