<template>
  <DataTable data-key="start" :value="rows" size="large">
    <Column field="start" header="Start">
      <template #body="{ data }">
        {{ formatTimeslot(data.start) }}
      </template>
    </Column>
    <Column field="timeslot.performer.name" header="Performer" />
    <Column field="timeslot.performanceType" header="Type" />
    <Column field="timeslot.name" header="Name" />
    <Column class="grid-action-col">
      <template #body="{ data }">
        <Button
          class="grid-action-col__item"
          :icon="`pi ${data.timeslot ? 'pi-pencil' : 'pi-plus'}`"
          :severity="data.timeslot ? 'secondary' : 'primary'"
          :disabled="isDialogOpen || props.disabled"
          @click="handleEditClicked(data)"
          outlined
          rounded
        />
        <Button
          class="grid-action-col__item"
          v-if="data.timeslot"
          icon="pi pi-trash"
          severity="danger"
          :disabled="isDialogOpen || props.disabled"
          @click="handleDeleteClicked(data)"
          outlined
          rounded
        />
      </template>
    </Column>
  </DataTable>
  <FormDialog
    :header="editingId ? 'Edit Timeslot' : 'Create Timeslot'"
    :is-submitting="isSubmitting"
    :visible="showFormDialog"
    @close="showFormDialog = false"
    @save="handleSave"
  >
    <TimeslotForm
      ref="timeslotForm"
      :initial-state="formInitialValue"
      :performers="performers"
      :disabled="isSubmitting"
    />
  </FormDialog>
  <DeleteDialog
    entity-type="timeslot"
    header="Delete Timeslot"
    :entity-name="deletingName"
    :visible="showDeleteDialog"
    :is-submitting="false"
    @close="showDeleteDialog = false"
    @delete="handleDelete"
  />
</template>

<script lang="ts" setup>
import api from '@/api/api'
import type { ApiResponse } from '@/api/apiResponse'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import { PerformanceType } from '@/api/schedules/timeslots/performanceType'
import type { TimeslotRequest } from '@/api/schedules/timeslots/timeslotRequest'
import type { TimeslotResponse } from '@/api/schedules/timeslots/timeslotResponse'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import TimeslotForm from '@/components/form/requestForms/TimeslotForm.vue'
import { formatTimeslot, getNextHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { Button, Column, DataTable, useToast } from 'primevue'
import { computed, onMounted, ref, useTemplateRef, watch, type Ref } from 'vue'

const toast = useToast()
const props = defineProps<{
  schedule: ScheduleResponse
  performers: PerformerResponse[]
  disabled: boolean
}>()

const timeslots: Ref<TimeslotResponse[]> = ref(props.schedule.timeslots)
const startDate = ref(parseDate(props.schedule.start))
const endDate = ref(parseDate(props.schedule.end))
const isSubmitting = ref(false)

interface TimeslotRow {
  start: Date
  isEditing: boolean
  timeslot?: TimeslotResponse
}

onMounted(() => {
  setupRows()
})

const rows: Ref<TimeslotRow[]> = ref([])
const setupRows = () => {
  const newRows: TimeslotRow[] = []
  const hours = hoursBetween(startDate.value, endDate.value)
  hours.forEach((hour) => {
    newRows.push({
      start: hour,
      isEditing: false,
      timeslot: timeslots.value.find((t) => Date.parse(t.start) === hour.getTime())
    })
  })
  rows.value = newRows
}
const updateRows = async () => {
  timeslots.value = (await api.timeslots.get(props.schedule.id)).data ?? props.schedule.timeslots
  setupRows()
}

const showFormDialog = ref(false)
let editingId = ''
const formInitialValue: Ref<TimeslotRequest> = ref({
  start: '',
  end: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: ''
})
const timeslotForm = useTemplateRef('timeslotForm')
const handleEditClicked = (row: TimeslotRow) => {
  showFormDialog.value = true
  editingId = row.timeslot?.id ?? ''
  formInitialValue.value = {
    start: row.start.toISOString(),
    end: getNextHour(row.start).toISOString(),
    performerId: row.timeslot?.performer.id ?? '',
    performanceType: row.timeslot?.performanceType ?? PerformanceType.Live,
    name: row.timeslot?.performanceType ?? ''
  }
}
const handleSave = async () => {
  if (!timeslotForm.value) return
  const isValid = timeslotForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const request = timeslotForm.value.formState
  let response: ApiResponse<TimeslotRequest, never> | undefined = undefined
  if (editingId) {
    response = await api.timeslots.put(props.schedule.id, editingId, request)
  } else {
    response = await api.timeslots.post(props.schedule.id, request)
  }
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(response, toast, timeslotForm.value.validation)) return

  const requestPerformer = props.performers.find((p) => p.id === request.performerId)
  if (editingId) {
    showEditSuccessToast(
      toast,
      'timeslot',
      `${formatTimeslot(parseDate(request.start))} | ${requestPerformer!.name}`
    )
  } else {
    showCreateSuccessToast(
      toast,
      'timeslot',
      `${formatTimeslot(parseDate(request.start))} | ${requestPerformer!.name}`
    )
  }
  updateRows()
  editingId = ''
  showFormDialog.value = false
}

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteClicked = async (row: TimeslotRow) => {
  if (!row.timeslot) return
  deletingId = row.timeslot!.id
  deletingName.value = `${formatTimeslot(parseDate(row.timeslot.start))} - ${row.timeslot.performer.name}`
  showDeleteDialog.value = true
}
const handleDelete = async () => {
  isSubmitting.value = true
  const response = await api.timeslots.delete(props.schedule.id, deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  showDeleteSuccessToast(toast, 'timeslot')
  showDeleteDialog.value = false
  deletingId = ''
  deletingName.value = ''
  updateRows()
}

const emit = defineEmits<{ dialogStateChange: [value: boolean] }>()
const isDialogOpen = computed(() => showDeleteDialog.value || showFormDialog.value)
watch(
  () => isDialogOpen.value,
  (newValue) => {
    emit('dialogStateChange', newValue)
  }
)
</script>
