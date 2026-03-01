<template>
  <div class="soundclash-grid">
    <MarkdownContent
      v-if="schedule.description"
      :content="schedule.description" />
    <DataTable
      v-if="!isMobile"
      :value="rows"
      size="large">
      <Column>
        <template #body="{ data }: { data: SoundclashRow }">
          <SlotTime :date="data.start" />
        </template>
      </Column>
      <Column>
        <template #body="{ data }: { data: SoundclashRow }">
          <TwoLineData
            v-if="data.soundclash"
            :above="`${data.soundclash.performerOne.name} vs. ${data.soundclash.performerTwo.name}`"
            :below="`${data.soundclash.roundOne} | ${data.soundclash.roundTwo} | ${data.soundclash.roundThree}`" />
        </template>
      </Column>
      <Column class="grid-action-col grid-action-col--2">
        <template #body="{ data }: { data: SoundclashRow }">
          <GridActions
            :show-create="schedule.isSoundclashCreationAllowed && data.soundclash === undefined"
            :show-delete="data.soundclash?.isDeletable"
            :show-edit="data.soundclash?.isEditable"
            @create="handleCreate(data)"
            @delete="handleDelete(data)"
            @edit="handleEdit(data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-if="isMobile"
      :rows="rows.length"
      :value="rows">
      <template #list="{ items }: { items: SoundclashRow[] }">
        <div
          v-for="(data, index) in items"
          :key="data.start.toISOString()"
          class="p-mb-3">
          <ListItem class="soundclash-grid__item">
            <template #left>
              <div>{{ formatReadableTime(data.start) }}</div>
              <div>
                {{
                  data.soundclash
                    ? `${data.soundclash?.performerOne.name} vs. ${data.soundclash?.performerTwo.name}`
                    : ''
                }}
              </div>
            </template>
            <template #right>
              <GridActions
                :show-create="schedule.isSoundclashCreationAllowed && data.soundclash === undefined"
                :show-delete="data.soundclash?.isDeletable"
                :show-edit="data.soundclash?.isEditable"
                @create="handleCreate(data)"
                @delete="handleDelete(data)"
                @edit="handleEdit(data)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
    </DataView>
    <FormDrawer
      v-if="selectedRow !== undefined"
      v-model:visible="showSoundclashForm"
      :is-submitting="soundclashFormRef?.isSubmitting"
      :title="selectedRow.soundclash ? 'Create Soundclash' : 'Edit Soundclash'"
      @reset="soundclashFormRef?.reset()"
      @submit="soundclashFormRef?.submit()">
      <SoundclashForm
        ref="soundclashFormRef"
        :schedule-id="schedule.id"
        :soundclash="selectedRow.soundclash"
        :start="selectedRow.start"
        @submitted="showSoundclashForm = false" />
    </FormDrawer>
    <DeleteDialog
      v-model:visible="showDeleteDialog"
      :entity-name="
        selectedRow?.soundclash?.performerOne.name +
        ' vs. ' +
        selectedRow?.soundclash?.performerTwo.name
      "
      :is-submitting="isDeleteSubmitting"
      entity-type="Soundclash"
      header="Delete Soundclash"
      @delete="handleConfirmDelete"
      @hide="showDeleteDialog = false" />
  </div>
</template>

<script lang="ts" setup>
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import { Column, DataTable, DataView, Divider } from 'primevue'
import type { SoundclashResponse } from '@/api/resources/soundclashApi.ts'
import { inject, onMounted, type Ref, ref, useTemplateRef, watch } from 'vue'
import { formatReadableTime, parseDate, parseTime, timesBetween } from '@/utils/dateUtils.ts'
import SlotTime from '@/components/controls/SlotTime.vue'
import TwoLineData from '@/components/layout/TwoLineData.vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import SoundclashForm from '@/components/form/requestForms/SoundclashForm.vue'
import DeleteDialog from '@/components/dialogs/DeleteDialog.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import ListItem from '@/components/data/ListItem.vue'
import MarkdownContent from '@/components/controls/MarkdownContent.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const soundclashFormRef = useTemplateRef('soundclashFormRef')
const schedules = useScheduleStore()

const props = defineProps<{
  schedule: ScheduleResponse
}>()

interface SoundclashRow {
  start: Date
  soundclash?: SoundclashResponse
}

const rows: Ref<SoundclashRow[]> = ref([])

const setupRows = () => {
  const newRows: SoundclashRow[] = []
  const rowTimes = timesBetween(
    parseDate(props.schedule.startsAt),
    parseDate(props.schedule.endsAt),
    120
  )
  rowTimes.forEach((time) => {
    const soundclash = props.schedule.soundclashes.find(
      (sc) => parseTime(sc.startsAt) === time.getTime()
    )
    newRows.push({
      start: time,
      soundclash: soundclash
    })
  })
  rows.value = newRows
}

watch(
  () => props.schedule,
  () => {
    setupRows()
  },
  { deep: true }
)

const selectedRow: Ref<SoundclashRow | undefined> = ref(undefined)
const showSoundclashForm = ref(false)
const handleCreate = (row: SoundclashRow) => {
  selectedRow.value = row
  showSoundclashForm.value = true
}
const handleEdit = (row: SoundclashRow) => {
  selectedRow.value = row
  showSoundclashForm.value = true
}

const showDeleteDialog = ref(false)
const isDeleteSubmitting = ref(false)
const handleDelete = (row: SoundclashRow) => {
  selectedRow.value = row
  showDeleteDialog.value = true
}
const handleConfirmDelete = async () => {
  if (!selectedRow.value?.soundclash) return
  isDeleteSubmitting.value = true
  const result = await schedules.deleteSoundclash(selectedRow.value.soundclash.id)
  showDeleteDialog.value = false
  if (result.isSuccess) {
    showDeleteDialog.value = false
  }
}

onMounted(() => setupRows())
</script>

<style lang="scss" scoped>
@use '@/assets/styles/variables';
.soundclash-grid {
  &__item {
    padding: variables.$space-m 0;
  }

  &__schedule-description {
    font-size: medium;
  }
}
</style>
