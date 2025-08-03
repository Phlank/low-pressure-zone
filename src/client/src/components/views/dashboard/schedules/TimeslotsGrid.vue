<template>
  <div class="timeslots-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="rows"
        size="large">
        <Column
          field="start"
          header="Start">
          <template #body="{ data }: { data: TimeslotRow }">
            {{ formatReadableTime(data.start) }}
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
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: TimeslotRow }">
            <GridActions
              :show-create="
                schedule.isTimeslotCreationAllowed &&
                data.timeslot === undefined &&
                data.start.getTime() > new Date().getTime()
              "
              :show-delete="data.timeslot?.isDeletable"
              :show-edit="data.timeslot?.isEditable"
              @create="handleEditClicked(data)"
              @delete="handleDeleteClicked(data)"
              @edit="handleEditClicked(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <div v-else>
      <ListItem
        v-for="row in rows"
        :key="row.start.getTime()"
        class="timeslots-grid__item">
        <template #left>
          <div>{{ formatReadableTime(row.start) }}</div>
          <div>{{ row.timeslot?.performer.name ?? '' }}</div>
        </template>
        <template #right>
          <GridActions
            :show-create="
              schedule.isTimeslotCreationAllowed &&
              row.timeslot === undefined &&
              row.start.getTime() > new Date().getTime()
            "
            :show-delete="row.timeslot?.isDeletable"
            :show-edit="row.timeslot?.isEditable"
            @create="handleEditClicked(row)"
            @delete="handleDeleteClicked(row)"
            @edit="handleEditClicked(row)" />
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
        :disabled="isSubmitting"
        :initial-state="formInitialValue"
        :performers="performerStore.linkablePerformers" />
    </FormDialog>
    <DeleteDialog
      :entity-name="deletingName"
      :is-submitting="false"
      :visible="showDeleteDialog"
      entity-type="timeslot"
      header="Delete Timeslot"
      @close="showDeleteDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import type { ApiResponse } from '@/api/apiResponse'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import TimeslotForm from '@/components/form/requestForms/TimeslotForm.vue'
import { formatReadableTime, getNextHour, hoursBetween, parseDate } from '@/utils/dateUtils'
import {
  showCreateSuccessToast,
  showDeleteSuccessToast,
  showEditSuccessToast
} from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { computed, inject, onMounted, ref, type Ref, useTemplateRef } from 'vue'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import timeslotsApi, {
  PerformanceType,
  type TimeslotRequest,
  type TimeslotResponse
} from '@/api/resources/timeslotsApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { usePerformerStore } from '@/stores/performerStore.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const toast = useToast()
const props = defineProps<{
  schedule: ScheduleResponse
}>()

const scheduleStore = useScheduleStore()
const performerStore = usePerformerStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const timeslots = computed(() => props.schedule.timeslots)
const startDate = computed(() => parseDate(props.schedule.startsAt))
const endDate = computed(() => parseDate(props.schedule.endsAt))
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
      timeslot: timeslots.value.find((t) => Date.parse(t.startsAt) === hour.getTime())
    })
  })
  rows.value = newRows
}

onMounted(() => {
  setupRows()
})

const rows: Ref<TimeslotRow[]> = ref([])

const showFormDialog = ref(false)
let editingId = ''
const formInitialValue: Ref<TimeslotRequest> = ref({
  startsAt: '',
  endsAt: '',
  performerId: '',
  performanceType: PerformanceType.Live,
  name: ''
})
const timeslotForm = useTemplateRef('timeslotForm')
const handleEditClicked = (row: TimeslotRow) => {
  showFormDialog.value = true
  editingId = row.timeslot?.id ?? ''
  formInitialValue.value = {
    startsAt: row.start.toISOString(),
    endsAt: getNextHour(row.start).toISOString(),
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
  let response: ApiResponse<TimeslotRequest, never> | undefined
  if (editingId) {
    response = await timeslotsApi.put(props.schedule.id, editingId, request)
  } else {
    response = await timeslotsApi.post(props.schedule.id, request)
  }
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(response, toast, timeslotForm.value.validation)) return

  showFormDialog.value = false
  editingId = ''

  const performer = performerStore.get(request.performerId)
  if (editingId) {
    showEditSuccessToast(
      toast,
      'timeslot',
      `${formatReadableTime(parseDate(request.startsAt))} | ${performer!.name}`
    )
  } else {
    showCreateSuccessToast(
      toast,
      'timeslot',
      `${formatReadableTime(parseDate(request.startsAt))} | ${performer!.name}`
    )
  }
  await scheduleStore.reloadTimeslotsAsync(props.schedule.id)
  setupRows()
}

const showDeleteDialog = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteClicked = async (row: TimeslotRow) => {
  if (!row.timeslot) return
  deletingId = row.timeslot!.id
  deletingName.value = `${formatReadableTime(parseDate(row.timeslot.startsAt))} - ${row.timeslot.performer.name}`
  showDeleteDialog.value = true
}
const handleDelete = async () => {
  isSubmitting.value = true
  const response = await timeslotsApi.delete(props.schedule.id, deletingId)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  await scheduleStore.reloadTimeslotsAsync(props.schedule.id)
  setupRows()
  showDeleteSuccessToast(toast, 'timeslot')
  showDeleteDialog.value = false
  deletingId = ''
  deletingName.value = ''
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.timeslots-grid {
  &__item {
    padding: variables.$space-m 0;
  }
}
</style>
