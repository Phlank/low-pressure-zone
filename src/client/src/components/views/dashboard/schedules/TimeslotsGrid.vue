<template>
  <div class="timeslots-grid">
    <p class="timeslots-grid__schedule-description">{{ schedule?.description }}</p>
    <DataTable
      v-if="!isMobile"
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
              schedule?.isTimeslotCreationAllowed &&
              data.timeslot === undefined &&
              data.start.getTime() > new Date().getTime()
            "
            :show-delete="data.timeslot?.isDeletable && data.isFirstRowOfTimeslot"
            :show-edit="data.timeslot?.isEditable && data.isFirstRowOfTimeslot"
            @create="handleCreateClicked(data)"
            @delete="handleDeleteAction(data)"
            @edit="handleEditClicked(data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-else
      :value="rows"
      data-key="timeslot.id">
      <template #list="{ items }: { items: TimeslotRow[] }">
        <div
          v-for="(row, index) in items"
          :key="row.start.getTime()">
          <ListItem class="timeslots-grid__item">
            <template #left>
              <div>{{ formatReadableTime(row.start) }}</div>
              <div>{{ row.timeslot?.performer.name ?? '' }}</div>
            </template>
            <template #right>
              <GridActions
                :show-create="
                  schedule?.isTimeslotCreationAllowed &&
                  row.timeslot === undefined &&
                  row.start.getTime() > new Date().getTime()
                "
                :show-delete="row.timeslot?.isDeletable && row.isFirstRowOfTimeslot"
                :show-edit="row.timeslot?.isEditable && row.isFirstRowOfTimeslot"
                @create="handleCreateClicked(row)"
                @delete="handleDeleteAction(row)"
                @edit="handleEditClicked(row)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
    </DataView>
    <FormDrawer
      v-model:visible="showTimeslotForm"
      :is-submitting="timeslotFormRef?.isSubmitting"
      :title="currentTimeslotRow?.timeslot ? 'Edit Timeslot' : 'Create Timeslot'"
      @reset="timeslotFormRef?.reset()"
      @submit="timeslotFormRef?.submit()">
      <TimeslotForm
        ref="timeslotFormRef"
        :schedule-id="schedule.id"
        :start="currentTimeslotRow?.start ?? new Date()"
        :timeslot="currentTimeslotRow?.timeslot"
        @submitted="showTimeslotForm = false" />
    </FormDrawer>
    <DeleteDialog
      :entity-name="deletingName"
      :is-submitting="false"
      :visible="showDeleteDialog"
      entity-type="timeslot"
      header="Delete Timeslot"
      @delete="handleDeleteConfirm"
      @hide="showDeleteDialog = false" />
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import TimeslotForm from '@/components/form/requestForms/TimeslotForm.vue'
import {
  formatReadableTime,
  hoursBetween,
  isDateInTimeslot,
  parseDate,
  parseTime
} from '@/utils/dateUtils'
import { Column, DataTable, DataView, Divider } from 'primevue'
import { computed, inject, onMounted, ref, type Ref, useTemplateRef, watch } from 'vue'
import { type TimeslotResponse } from '@/api/resources/timeslotsApi.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import SlotTime from '@/components/controls/SlotTime.vue'
import SlotName from '@/components/controls/SlotName.vue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'

const props = defineProps<{
  schedule: ScheduleResponse
}>()

const schedules = useScheduleStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const timeslots = computed(() => props.schedule.timeslots)
const startDate = computed(() => parseDate(props.schedule.startsAt ?? new Date()))
const endDate = computed(() => parseDate(props.schedule.endsAt ?? new Date()))
const timeslotFormRef = useTemplateRef('timeslotFormRef')

interface TimeslotRow {
  start: Date
  isFirstRowOfTimeslot: boolean
  timeslot?: TimeslotResponse
}

watch(
  () => props.schedule,
  () => {
    setupRows()
  }, { deep: true }
)

const setupRows = () => {
  const newRows: TimeslotRow[] = []
  const hours = hoursBetween(startDate.value, endDate.value)
  hours.forEach((hour) => {
    const timeslot = timeslots.value?.find((timeslot) => isDateInTimeslot(hour, timeslot))
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

const showTimeslotForm = ref(false)
const currentTimeslotRow: Ref<TimeslotRow | undefined> = ref(undefined)
const handleCreateClicked = (row: TimeslotRow) => {
  currentTimeslotRow.value = row
  timeslotFormRef.value?.reset()
  showTimeslotForm.value = true
}
const handleEditClicked = (row: TimeslotRow) => {
  if (row.timeslot === undefined) return
  currentTimeslotRow.value = row
  timeslotFormRef.value?.reset()
  showTimeslotForm.value = true
}

const showDeleteDialog = ref(false)
const isDeleteSubmitting = ref(false)
let deletingId = ''
const deletingName = ref('')
const handleDeleteAction = async (row: TimeslotRow) => {
  if (!row.timeslot) return
  deletingId = row.timeslot!.id
  deletingName.value = `${formatReadableTime(parseDate(row.timeslot.startsAt))} - ${row.timeslot.performer.name}`
  showDeleteDialog.value = true
}
const handleDeleteConfirm = async () => {
  isDeleteSubmitting.value = true
  const result = await schedules.removeTimeslot(deletingId)
  isDeleteSubmitting.value = false
  if (!result.isSuccess) return
  showDeleteDialog.value = false
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
