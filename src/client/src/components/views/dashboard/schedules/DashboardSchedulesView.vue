<template>
  <div class="dashboard-schedules-view">
    <Tabs
      v-model:value="tabValue"
      scrollable>
      <TabList>
        <Tab value="upcoming">Upcoming</Tab>
        <Tab value="past">Past</Tab>
      </TabList>
      <TabPanels v-if="!schedules.isLoading">
        <TabPanel value="upcoming">
          <SchedulesGrid
            :schedules="schedules.upcomingSchedules"
            @create="handleCreate"
            @delete="handleDeleteAction"
            @edit="handleEdit" />
        </TabPanel>
        <TabPanel value="past">
          <SchedulesGrid
            :schedules="schedules.pastSchedules"
            hide-actions />
        </TabPanel>
      </TabPanels>
      <Skeleton
        v-if="schedules.isLoading"
        style="height: 300px" />
      <FormDrawer
        v-model:visible="showFormDrawer"
        :is-submitting="formRef?.isSubmitting"
        :title="editingSchedule ? 'Edit Schedule' : 'Create Schedule'"
        @reset="formRef?.reset"
        @submit="formRef?.submit">
        <ScheduleForm
          ref="formRef"
          :availableCommunities="communities.organizableCommunities"
          :schedule="editingSchedule"
          @submitted="showFormDrawer = false" />
      </FormDrawer>
      <DeleteDialog
        v-model:visible="showDeleteDialog"
        :entity-name="parseDate(deletingSchedule?.endsAt ?? new Date()).toLocaleString()"
        :is-submitting="false"
        entity-type="schedule"
        header="Delete Schedule"
        @delete="handleConfirmDelete"
        @hide="showDeleteDialog = false" />
    </Tabs>
  </div>
</template>

<script lang="ts" setup>
import { Skeleton, Tab, TabList, TabPanel, TabPanels, Tabs } from 'primevue'
import { ref, type Ref, useTemplateRef } from 'vue'
import SchedulesGrid from './SchedulesGrid.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import FormDrawer from '@/components/form/FormDrawer.vue'
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import ScheduleForm from '@/components/form/requestForms/ScheduleForm.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import { parseDate } from '@/utils/dateUtils.ts'

const schedules = useScheduleStore()
const communities = useCommunityStore()
const tabValue: Ref<string | number> = ref('upcoming')

const showFormDrawer = ref(false)
const editingSchedule: Ref<ScheduleResponse | undefined> = ref(undefined)
const formRef = useTemplateRef('formRef')
const handleCreate = () => {
  editingSchedule.value = undefined
  showFormDrawer.value = true
}
const handleEdit = (schedule: ScheduleResponse) => {
  editingSchedule.value = schedule
  showFormDrawer.value = true
}

const showDeleteDialog = ref(false)
const isDeleteSubmitting = ref(false)
const deletingSchedule: Ref<ScheduleResponse | undefined> = ref(undefined)
const handleDeleteAction = (schedule: ScheduleResponse) => {
  deletingSchedule.value = schedule
  showDeleteDialog.value = true
}
const handleConfirmDelete = async () => {
  isDeleteSubmitting.value = true
  const result = await schedules.removeSchedule(deletingSchedule.value!.id)
  isDeleteSubmitting.value = false
  if (!result.isSuccess) return
  showDeleteDialog.value = false
  deletingSchedule.value = undefined
}
</script>
