<template>
  <DataTable data-key="start" :value="rows" size="large">
    <Column field="start" header="Start">
      <template #body="{ data }">
        {{ formatHourOnly(data.start) }}
      </template>
    </Column>
    <Column field="timeslot.performer.name" header="Performer">
      <template #body="{ data }">
        <span v-if="!data.isEditing">{{ data.timeslot?.performer.name }}</span>
        <IftaLabel v-else class="input input--small">
          <Select
            class="input__field"
            id="timeslotPerformerSelect"
            option-label="name"
            option-value="id"
            :disabled="isSubmitting"
            :options="performers"
            :model-value="data.formState?.performerId"
            :invalid="!data.validation?.isValid('performerId')"
          />
          <ValidationLabel
            for="timeslotPerformerSelect"
            :message="data.validation?.message('performerId')"
          >
            Performer
          </ValidationLabel>
        </IftaLabel>
      </template>
    </Column>
    <Column field="timeslot.performanceType" header="Type">
      <template #body="{ data }">
        <span v-if="!data.isEditing">
          {{ data.timeslot?.performanceType }}
        </span>
        <IftaLabel v-else class="input input--small">
          <Select
            class="input__field"
            id="timeslotTypeSelect"
            :disabled="isSubmitting"
            :options="performanceTypes"
            :model-value="data.formState?.performanceType"
            @update:model-value="(value) => (data.formState.performanceType = value)"
            :invalid="!data.validation?.isValid('performanceType')"
          />
          <ValidationLabel
            for="timeslotTypeSelect"
            :message="data.validation?.message('performanceType')"
          >
            Performance Type
          </ValidationLabel>
        </IftaLabel>
      </template>
    </Column>
    <Column field="timeslot.name" header="Name">
      <template #body="{ data }">
        <span v-if="!data.isEditing">
          {{ data.timeslot?.name }}
        </span>
        <IftaLabel v-else class="input input--medium">
          <InputText
            class="input__field"
            id="timeslotNameInput"
            :disabled="isSubmitting"
            :model-value="data.formState?.name ?? ''"
            :invalid="!data.validation?.isValid('name')"
          />
          <ValidationLabel
            for="timeslotNameInput"
            :message="data.validation?.message('name')"
            optional
          >
            Name
          </ValidationLabel>
        </IftaLabel>
      </template>
    </Column>
    <Column style="width: 8.75rem; text-align: center">
      <template #body="{ data }">
        <span v-if="!data.isEditing">
          <Button
            class="action"
            :icon="`pi ${data.timeslot ? 'pi-pencil' : 'pi-plus'}`"
            :severity="data.timeslot ? 'secondary' : 'primary'"
            :disabled="isEditingRow"
            @click="handleEditClicked(data)"
            outlined
            rounded
          />
          <Button
            class="action"
            v-if="data.timeslot"
            icon="pi pi-trash"
            severity="danger"
            :disabled="isEditingRow"
            @click="handleDeleteClicked(data)"
            outlined
            rounded
          />
        </span>
        <span v-else>
          <Button
            class="action"
            icon="pi pi-times"
            severity="danger"
            @click="handleCancelClicked(data)"
            outlined
            rounded
          />
          <Button icon="pi pi-check" @click="handleSaveClicked(data)" outlined rounded />
        </span>
      </template>
    </Column>
  </DataTable>
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
import { DataTable, Column, Select, IftaLabel, useToast, InputText, Button } from 'primevue'
import type { ScheduleResponse } from '@/api/schedules/scheduleResponse'
import { formatHourOnly, getNextHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import { computed, onMounted, ref, type Ref } from 'vue'
import type { TimeslotResponse } from '@/api/schedules/timeslots/timeslotResponse'
import type { TimeslotRequest } from '@/api/schedules/timeslots/timeslotRequest'
import { createFormValidation, type FormValidation } from '@/validation/types/formValidation'
import type { PerformerResponse } from '@/api/performers/performerResponse'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { PerformanceType, performanceTypes } from '@/api/schedules/timeslots/performanceType'
import { timeslotRequestRules } from '@/validation/requestRules'
import type { ApiResponse } from '@/api/apiResponse'
import api from '@/api/api'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'

const toast = useToast()
const props = defineProps<{ schedule: ScheduleResponse; performers: PerformerResponse[] }>()

const timeslots: Ref<TimeslotResponse[]> = ref(props.schedule.timeslots)
const startDate = ref(parseDate(props.schedule.start))
const endDate = ref(parseDate(props.schedule.end))
const isSubmitting = ref(false)
const editingRows: Ref<unknown[]> = ref([])

const isEditingRow = computed(() => rows.value.some((r) => r.isEditing))

interface TimeslotRow {
  start: Date
  isEditing: boolean
  formState?: TimeslotRequest
  validation?: FormValidation<TimeslotRequest>
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

const handleEditClicked = (row: TimeslotRow) => {
  row.isEditing = true
  row.formState = {
    start: row.start.toISOString(),
    end: getNextHour(row.start).toISOString(),
    performerId: row.timeslot?.performer.id ?? getInitialCreatePerformerId(),
    performanceType: row.timeslot?.performanceType ?? PerformanceType.Live,
    name: row.timeslot?.name ?? null
  }
  row.validation = createFormValidation(row.formState, timeslotRequestRules(row.formState))
}

const getInitialCreatePerformerId = () => {
  if (props.performers.length == 1) {
    return props.performers[0].id
  }
  return ''
}

const handleCancelClicked = (row: TimeslotRow) => {
  row.isEditing = false
  row.validation = undefined
  row.formState = undefined
}

const handleSaveClicked = async (row: TimeslotRow) => {
  const isEdit = row.timeslot != undefined
  if (!row.formState || !row.validation) return
  const isValid = row.validation.validate()
  if (!isValid) return
  let response: ApiResponse<TimeslotRequest, never> | undefined = undefined
  if (isEdit) {
    response = await api.timeslots.put(props.schedule.id, row.timeslot!.id, row.formState)
  } else {
    response = await api.timeslots.post(props.schedule.id, row.formState)
  }

  if (tryHandleUnsuccessfulResponse(response, toast, row.validation)) {
    editingRows.value.push(row)
  }

  if (isEdit) {
    showEditSuccessToast(toast, 'timeslot', formatHourOnly(row.start))
  } else {
    showCreateSuccessToast(toast, 'timeslot', formatHourOnly(row.start))
  }
  timeslots.value = (await api.timeslots.get(props.schedule.id)).data ?? props.schedule.timeslots
  setupRows()
}

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteClicked = async (row: TimeslotRow) => {
  showDeleteDialog.value = true
  deletingId = row.timeslot!.id
  deletingName.value = formatHourOnly(parseDate(row.timeslot!.start))
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
  timeslots.value = (await api.timeslots.get(props.schedule.id)).data ?? props.schedule.timeslots
  setupRows()
}
</script>
