<template>
  <div class="soundclash-grid">
    <p v-if="schedule.description">{{ schedule.description }}</p>
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
  </div>
</template>

<script lang="ts" setup>
import type { ScheduleResponse } from '@/api/resources/schedulesApi.ts'
import { Column, DataTable } from 'primevue'
import type { SoundclashResponse } from '@/api/resources/soundclashApi.ts'
import { inject, onMounted, type Ref, ref, useTemplateRef, watch } from 'vue'
import { parseDate, parseTime, timesBetween } from '@/utils/dateUtils.ts'
import SlotTime from '@/components/controls/SlotTime.vue'
import TwoLineData from '@/components/layout/TwoLineData.vue'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import FormDrawer from '@/components/form/FormDrawer.vue'
import SoundclashForm from '@/components/form/requestForms/SoundclashForm.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const soundclashFormRef = useTemplateRef('soundclashFormRef')

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
  console.log(rows)
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

const handleDelete = (row: SoundclashRow) => {}

onMounted(() => setupRows())
</script>
