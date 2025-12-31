<template>
  <div class="schedules-grid">
    <div>
      <DataTable
        :expanded-rows="expandedRows"
        :value="schedules"
        class="schedules-grid__table"
        data-key="id">
        <template #empty> No items to display.</template>
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
              {{ formatReadableTime(parseDate(data.startsAt)) }} -
              {{ formatReadableTime(parseDate(data.endsAt)) }}
            </template>
          </Column>
          <Column v-if="isMobile">
            <template #body="{ data }: { data: ScheduleResponse }">
              <div>{{ parseDate(data.startsAt).toLocaleDateString() }}</div>
              <div class="text-s ellipsis">{{ data.community.name }}</div>
            </template>
          </Column>
          <!-- Only show the action col for grids with schedules in the future -->
          <Column
            v-if="showActionColumn"
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
            :schedule="rowProps.data" />
        </template>
      </DataTable>
    </div>
    <Dialog
      v-model:visible="showEditScheduleDialog"
      :draggable="false"
      :is-submitting="false"
      header="Edit Schedule"
      modal>
      <ScheduleForm
        :communities="communityStore.communities.filter((community) => community.isOrganizable)"
        :initial-state="editingSchedule"
        :schedule-id="editingId"
        align-actions="right"
        @after-submit="afterEditSubmit" />
    </Dialog>
    <DeleteDialog
      :is-submitting="false"
      :visible="showDeleteScheduleDialog"
      entity-type="schedule"
      header="Delete Schedule"
      @hide="showDeleteScheduleDialog = false"
      @delete="handleDelete" />
  </div>
</template>

<script lang="ts" setup>
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { formatReadableTime, parseDate, parseTime } from '@/utils/dateUtils'
import { Column, DataTable, Dialog, useToast } from 'primevue'
import { computed, inject, ref, type Ref } from 'vue'
import TimeslotsGrid from './TimeslotsGrid.vue'
import schedulesApi, {
  type ScheduleRequest,
  type ScheduleResponse
} from '@/api/resources/schedulesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const communityStore = useCommunityStore()
const scheduleStore = useScheduleStore()
const expandedRows = ref({})
const isMobile: Ref<boolean> | undefined = inject('isMobile')
const toast = useToast()

const props = defineProps<{
  schedules: ScheduleResponse[]
}>()

const showActionColumn = computed(() =>
  props.schedules.some(
    (schedule) =>
      parseTime(schedule.endsAt) > Date.now() && (schedule.isDeletable || schedule.isEditable)
  )
)

const showEditScheduleDialog = ref(false)
const editingId = ref('')
const editingSchedule: Ref<ScheduleRequest> = ref({
  communityId: '',
  startsAt: '',
  endsAt: '',
  description: ''
})

const handleEditScheduleActionClick = (schedule: ScheduleResponse) => {
  editingId.value = schedule.id
  editingSchedule.value = schedulesApi.mapResponseToRequest(schedule)
  showEditScheduleDialog.value = true
}

const afterEditSubmit = () => {
  showEditScheduleDialog.value = false
  scheduleStore.reloadTimeslotsAsync(editingId.value)
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
  scheduleStore.removeSchedule(deletingId)
}
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
