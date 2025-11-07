<template>
  <div class="timeslots-grid">
    <p class="timeslots-grid__schedule-description">{{ schedule.description }}</p>
    <div v-if="!isMobile">
      <DataTable
        :value="rows"
        size="large">
        <Column
          field="start"
          header="Start">
          <template #body="{ data }: { data: TimeslotRow }">
            <SlotTime :date="data.start" />
          </template>
        </Column>
        <Column
          field="timeslot.performer.name"
          header="Performer">
          <template #body="{ data }: { data: TimeslotRow }">
            <SlotName
              :name="data.timeslot?.name ?? ''"
              :performer="data.timeslot?.performer.name ?? ''" />
          </template>
        </Column>
        <Column
          field="timeslot.performanceType"
          header="Type" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: TimeslotRow }">
            <GridActions
              :show-create="
                schedule.isTimeslotCreationAllowed &&
                data.timeslot === undefined &&
                data.start.getTime() > new Date().getTime()
              "
              :show-delete="data.timeslot?.isDeletable && data.isFirstRowOfTimeslot"
              :show-edit="data.timeslot?.isEditable && data.isFirstRowOfTimeslot"
              @create="handleCreateClicked(data)"
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
            :show-delete="row.timeslot?.isDeletable && row.isFirstRowOfTimeslot"
            :show-edit="row.timeslot?.isEditable && row.isFirstRowOfTimeslot"
            @create="handleCreateClicked(row)"
            @delete="handleDeleteClicked(row)"
            @edit="handleEditClicked(row)" />
        </template>
      </ListItem>
    </div>
    <Dialog
      v-model:visible="showTimeslotDialog"
      :draggable="false"
      :header="timeslotDialogTitle"
      modal>
      <TimeslotForm
        ref="timeslotForm"
        :disabled="isSubmitting"
        :initial-state="formInitialValue"
        :performers="performerStore.linkablePerformers"
        :schedule-id="schedule.id"
        :timeslot-id="editingId"
        @after-submit="onSubmitted" />
    </Dialog>
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
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import TimeslotForm from '@/components/form/requestForms/TimeslotForm.vue'
import {
  formatReadableTime,
  getNextHour,
  hoursBetween,
  isDateInTimeslot,
  parseDate,
  parseTime
} from '@/utils/dateUtils'
import { showDeleteSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, Dialog, useToast } from 'primevue'
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
import SlotTime from '@/components/controls/SlotTime.vue'
import SlotName from '@/components/controls/SlotName.vue'

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
  isFirstRowOfTimeslot: boolean
  timeslot?: TimeslotResponse
}

const setupRows = () => {
  const newRows: TimeslotRow[] = []
  const hours = hoursBetween(startDate.value, endDate.value)
  hours.forEach((hour) => {
    const timeslot = timeslots.value.find((timeslot) => isDateInTimeslot(hour, timeslot))
    newRows.push({
      start: hour,
      isFirstRowOfTimeslot: timeslot ? parseTime(timeslot.startsAt) === hour.getTime() : false,
      timeslot: timeslot
    })
  })
  rows.value = newRows
}

onMounted(() => {
  setupRows()
})

const rows: Ref<TimeslotRow[]> = ref([])

const showTimeslotDialog = ref(false)
const timeslotDialogTitle = ref('')
const editingId = ref('')
const formInitialValue: Ref<TimeslotRequest> = ref({
  startsAt: '',
  endsAt: '',
  performerId: '',
  name: '',
  performanceType: PerformanceType.Live,
  performerName: ''
})
const timeslotForm = useTemplateRef('timeslotForm')

const handleCreateClicked = (row: TimeslotRow) => {
  if (row.timeslot !== undefined) return
  timeslotDialogTitle.value = 'Create Timeslot'
  formInitialValue.value = {
    startsAt: row.start.toISOString(),
    endsAt: getNextHour(row.start).toISOString(),
    name: '',
    performerId: '',
    performanceType: PerformanceType.Live
  }
  showTimeslotDialog.value = true
}

const handleEditClicked = (row: TimeslotRow) => {
  if (row.timeslot === undefined) return
  editingId.value = row.timeslot.id
  timeslotDialogTitle.value = 'Edit Timeslot'
  formInitialValue.value = {
    startsAt: row.start.toISOString(),
    endsAt: row.timeslot.endsAt,
    name: row.timeslot?.name ?? '',
    performerId: row.timeslot?.performer.id ?? '',
    performanceType: row.timeslot?.performanceType ?? PerformanceType.Live
  }
  showTimeslotDialog.value = true
}

const onSubmitted = async () => {
  showTimeslotDialog.value = false
  timeslotDialogTitle.value = ''
  editingId.value = ''
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

  &__schedule-description {
    font-size: medium;
  }
}
</style>
