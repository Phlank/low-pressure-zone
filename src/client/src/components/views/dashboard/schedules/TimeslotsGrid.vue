<template>
  <div class="timeslots-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="rows"
        size="large">
        <Column
          field="start"
          header="Start">
          <template #body="{ data }">
            {{ formatTimeslot(data.start) }}
          </template>
        </Column>
        <Column
          field="timeslot.performer.name"
          header="Performer" />
        <Column
          field="timeslot.performanceType"
          header="Type" />
        <Column
          field="timeslot.name"
          header="Name" />
        <Column class="grid-action-col">
          <template #body="{ data }">
            <GridActions
              :show-create="!data.timeslot"
              :show-edit="data.timeslot != undefined"
              :show-delete="data.timeslot != undefined"
              @create="handleEditClicked(data)"
              @edit="handleEditClicked(data)"
              @delete="handleDeleteClicked(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <ListItem
        class="timeslots-grid__item"
        v-for="row in rows"
        :key="row.start.getTime()">
        <template #left>
          <div>{{ formatTimeslot(row.start) }}</div>
          <div>{{ row.timeslot?.performer.name ?? '' }}</div>
        </template>
        <template #right>
          <GridActions
            :show-create="row.timeslot == undefined"
            :show-edit="row.timeslot != undefined"
            :show-delete="row.timeslot != undefined"
            @create="handleEditClicked(row)"
            @edit="handleEditClicked(row)"
            @delete="handleDeleteClicked(row)" />
        </template>
      </ListItem>
    </div>
    <FormDialog
      :header="editingId ? 'Edit Timeslot' : 'Create Timeslot'"
      :is-submitting="isSubmitting"
      :visible="showFormDialog"
      @close="showFormDialog = false"
      @save="handleSave">
      <TimeslotForm
        ref="timeslotForm"
        :initial-state="formInitialValue"
        :performers="performers"
        :disabled="isSubmitting" />
    </FormDialog>
    <DeleteDialog
      entity-type="timeslot"
      header="Delete Timeslot"
      :entity-name="deletingName"
      :visible="showDeleteDialog"
      :is-submitting="false"
      @close="showDeleteDialog = false"
      @delete="handleDelete" />
  </div>
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
import GridActions from '@/components/data/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import TimeslotForm from '@/components/form/requestForms/TimeslotForm.vue'
import { formatTimeslot, getNextHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { inject, onMounted, onUpdated, ref, useTemplateRef, watch, type Ref } from 'vue'

const toast = useToast()
const props = defineProps<{
  schedule: ScheduleResponse
  performers: PerformerResponse[]
}>()

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const timeslots: Ref<TimeslotResponse[]> = ref(props.schedule.timeslots)
const startDate = ref(parseDate(props.schedule.start))
const endDate = ref(parseDate(props.schedule.end))
const isSubmitting = ref(false)

interface TimeslotRow {
  start: Date
  isEditing: boolean
  timeslot?: TimeslotResponse
}

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

onMounted(() => {
  setupRows()
})

watch(
  () => props.schedule,
  () => setupRows(),
  { deep: true }
)

const rows: Ref<TimeslotRow[]> = ref([])

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
    name: row.timeslot?.name ?? ''
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

  showFormDialog.value = false

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

  const timeslotResponse = await api.timeslots.get(props.schedule.id)
  if (timeslotResponse.isSuccess()) {
    timeslots.value = timeslotResponse.data!
  }

  emit('update')
  editingId = ''
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
  const index = timeslots.value.findIndex((t) => t.id === deletingId)
  timeslots.value.splice(index, 1)

  deletingId = ''
  deletingName.value = ''
  emit('update')
}

const emit = defineEmits<{ update: [] }>()
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.timeslots-grid {
  &__item {
    padding: variables.$space-m 0;
  }
}
</style>
