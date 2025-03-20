<template>
  <div class="schedules-grid">
    <div>
      <DataTable
        :expanded-rows="expandedRows"
        :sort-order="1"
        :value="schedules"
        class="schedules-grid__table"
        data-key="id"
        sort-field="start">
        <template>
          <Column
            expander
            style="width: fit-content" />
          <Column
            v-if="!isMobile"
            field="community.name"
            header="Community" />
          <Column
            v-if="!isMobile"
            :sortable="true"
            field="start"
            header="Date">
            <template #body="{ data }: { data: ScheduleResponse }">
              {{ parseDate(data.startsAt).toLocaleDateString() }}
            </template>
          </Column>
          <Column
            v-if="!isMobile"
            field="end"
            header="Time">
            <template #body="{ data }: { data: ScheduleResponse }">
              {{ formatTimeslot(parseDate(data.startsAt)) }} -
              {{ formatTimeslot(parseDate(data.endsAt)) }}
            </template>
          </Column>
          <Column v-if="isMobile">
            <template #body="{ data }: { data: ScheduleResponse }">
              <div>{{ parseDate(data.startsAt).toLocaleDateString() }}</div>
              <div class="text-s ellipsis">{{ data.community.name }}</div>
            </template>
          </Column>
          <Column
            :class="'grid-action-col' + isMobile ? 'grid-action-col--1' : 'grid-action-col--2'">
            <template #body="{ data }: { data: ScheduleResponse }">
              <GridActions
                :show-delete="data.isDeletable"
                :show-edit="data.isEditable"
                @delete="handleDeleteScheduleActionClick(data)"
                @edit="handleEditScheduleActionClick(data)" />
            </template>
          </Column>
        </template>
        <template #expansion="rowProps">
          <TimeslotsGrid
            :disabled="false"
            :performers="performers"
            :schedule="rowProps.data"
            @update="emit('update', rowProps.data.id)" />
        </template>
      </DataTable>
    </div>
    <FormDialog
      :is-submitting="false"
      :visible="showEditScheduleDialog"
      header="Edit Schedule"
      @close="showEditScheduleDialog = false"
      @save="handleEditScheduleSave">
      <ScheduleForm
        ref="scheduleEditForm"
        :communities="communities"
        :initial-state="editScheduleFormInitialState" />
    </FormDialog>
    <DeleteDialog
      :is-submitting="false"
      :visible="showDeleteScheduleDialog"
      entity-type="schedule"
      header="Delete Schedule"
      @close="showDeleteScheduleDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import FormDialog from '@/components/dialogs/FormDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { formatTimeslot, parseDate } from '@/utils/dateUtils'
import { showEditSuccessToast } from '@/utils/toastUtils'
import { Column, DataTable, useToast } from 'primevue'
import { inject, reactive, ref, type Ref, useTemplateRef } from 'vue'
import TimeslotsGrid from './TimeslotsGrid.vue'
import schedulesApi, { type ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import type { PerformerResponse } from '@/api/resources/performersApi.ts'
import type { CommunityResponse } from '@/api/resources/communitiesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const expandedRows = ref({})
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const isSubmitting = ref(false)
const toast = useToast()

defineProps<{
  schedules: ScheduleResponse[]
  performers: PerformerResponse[]
  communities: CommunityResponse[]
}>()

const scheduleEditForm = useTemplateRef('scheduleEditForm')
const showEditScheduleDialog = ref(false)
let editingId = ''
const editScheduleFormInitialState = reactive({
  communityId: '',
  startsAt: '',
  endsAt: '',
  description: '',
  startTime: new Date(),
  endTime: new Date()
})

const handleEditScheduleActionClick = (schedule: ScheduleResponse) => {
  editingId = schedule.id
  editScheduleFormInitialState.communityId = schedule.community.id
  editScheduleFormInitialState.startsAt = schedule.startsAt
  editScheduleFormInitialState.endsAt = schedule.endsAt
  editScheduleFormInitialState.description = schedule.description
  editScheduleFormInitialState.startTime = parseDate(schedule.startsAt)
  editScheduleFormInitialState.endTime = parseDate(schedule.endsAt)
  showEditScheduleDialog.value = true
}
const handleEditScheduleSave = async () => {
  if (!scheduleEditForm.value) return
  const isValid = scheduleEditForm.value.validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await schedulesApi.put(editingId, scheduleEditForm.value.formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, scheduleEditForm.value.validation)) return
  showEditSuccessToast(toast, 'schedule')
  showEditScheduleDialog.value = false

  const newScheduleResponse = await schedulesApi.getById(editingId)
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
  const response = await schedulesApi.delete(deletingId)
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
