<template>
  <div class="slots-grid">
    <DataTable
      v-if="!isMobile"
      :value="rows"
      size="large">
      <Column
        field="start"
        header="Start">
        <template #body="{ data }: { data: Row }">
          <SlotTime :date="data.start" />
        </template>
      </Column>
      <Column>
        <template #body="{ data }: { data: Row }">
          <div v-if="data.slot?.type === 'Hourly'">
            <SlotName
              :is-prerecorded="data.slot.isPrerecorded"
              :name="data.slot.subtitle"
              :performer="getHourlyPerformerName(data.slot)" />
          </div>
          <div v-else-if="data.slot?.type === 'Clash'">
            <TwoLineData
              :above="getPerformersClashText(data.slot)"
              :below="getRoundsClashText(data.slot)" />
          </div>
        </template>
      </Column>
      <Column class="grid-action-col grid-action-col--2">
        <template #body="{ data }: { data: Row }">
          <GridActions
            :show-create="
              !data.slot &&
              (schedule.isHourlySlotCreationAllowed || schedule.isClashSlotCreationAllowed)
            "
            :show-delete="data.slot?.isDeletable"
            :show-edit="data.slot?.isEditable"
            @create="onCreateSlotClicked(data)"
            @delete="onDeleteSlotClicked(data)"
            @edit="onEditSlotClicked(data)" />
        </template>
      </Column>
    </DataTable>
    <DataView
      v-if="isMobile"
      :rows="rows.length"
      :value="rows">
      <template #list="{ items }: { items: Row[] }">
        <div
          v-for="(row, index) in items"
          :key="row.start.toISOString()"
          class="p-mb-3">
          <ListItem class="slot-grid__item">
            <template #left>
              <div>
                {{ formatReadableTime(row.start) }}
              </div>
              <div v-if="row.slot?.type === 'Hourly'">
                {{ getHourlyPerformerName(row.slot) }}
              </div>
              <div v-if="row.slot?.type === 'Clash'">
                {{ getPerformersClashText(row.slot) }}
              </div>
            </template>
            <template #right>
              <GridActions
                :show-create="
                  row.slot === undefined &&
                  (schedule.isHourlySlotCreationAllowed || schedule.isClashSlotCreationAllowed)
                "
                :show-delete="row.isFirstRowOfSlot && row.slot?.isDeletable"
                :show-edit="row.isFirstRowOfSlot && row.slot?.isEditable"
                @create="onCreateSlotClicked(row)"
                @delete="onDeleteSlotClicked(row)"
                @edit="onEditSlotClicked(row)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
        </div>
      </template>
    </DataView>
    <FormDrawer
      v-if="selectedRow !== undefined"
      :title="selectedRow?.slot === undefined ? 'Create Slot' : 'Edit Slot'"
      :is-submitting="slotForm?.isSubmitting"
      v-model:visible="showSlotFormDrawer"
      @submit="slotForm?.submit()"
      @reset="slotForm?.reset()">
      <SlotForm
        ref="slotFormComponent"
        :schedule-id="schedule.id"
        :start="selectedRow?.start ?? new Date()"
        :existing-slot="selectedRow?.slot"
        @submitted="showSlotFormDrawer = false" />
    </FormDrawer>
  </div>
</template>

<script lang="ts" setup>
import { Column, DataTable, DataView, Divider } from 'openvue'
import type { ScheduleResponse, SlotResponse } from '@/api/resources/schedulesApi.ts'
import { inject, onMounted, onUnmounted, type Ref, ref, useTemplateRef, watch } from 'vue'
import {
  formatReadableTime,
  isDateInSlot,
  parseDate,
  parseTime,
  timesBetween
} from '@/utils/dateUtils.ts'
import SlotTime from '@/components/controls/SlotTime.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import SlotName from '@/components/controls/SlotName.vue'
import TwoLineData from '@/components/layout/TwoLineData.vue'
import type { ClashSlotResponse } from '@/api/resources/clashSlotsApi.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import ListItem from '@/components/data/ListItem.vue'
import type { HourlySlotResponse } from '@/api/resources/hourlySlotsApi.ts'
import FormDrawer from '@/components/form/FormDrawer.vue'
import SlotForm from '@/components/form/requestForms/SlotForm.vue'

const isMobile = inject<boolean>('isMobile')
const performers = usePerformerStore()
const slotForm = useTemplateRef('slotFormComponent')
const props = defineProps<{ schedule: ScheduleResponse }>()

interface Row {
  start: Date
  isFirstRowOfSlot: boolean
  slot?: SlotResponse
}

const rows: Ref<Row[]> = ref([])
const selectedRow: Ref<Row | undefined> = ref(undefined)
const setupRows = () => {
  const newRows: Row[] = []
  const rowTimes = timesBetween(
    parseDate(props.schedule.startsAt),
    parseDate(props.schedule.endsAt),
    60
  )
  rowTimes.forEach((time) => {
    const slot = props.schedule.slots.find((s) => isDateInSlot(time, s))
    newRows.push({
      start: time,
      isFirstRowOfSlot: slot ? parseTime(slot.startsAt) === time.getTime() : false,
      slot: slot
    })
  })
  rows.value = newRows
}
watch(
  () => props.schedule,
  () => {
    setupRows()
  },
  { immediate: true, deep: true }
)

const getHourlyPerformerName = (hourly: HourlySlotResponse) => {
  return performers.getById(hourly.performerId)?.name ?? ''
}

const getPerformersClashText = (clash: ClashSlotResponse) => {
  return `${performers.getById(clash.performerOneId)?.name ?? ''} vs. ${performers.getById(clash.performerTwoId)?.name ?? ''}`
}
const getRoundsClashText = (clash: ClashSlotResponse) => {
  return clash.rounds?.join(' | ') ?? ''
}

const showSlotFormDrawer = ref(false)
const onCreateSlotClicked = (row: Row) => {
  selectedRow.value = row
  showSlotFormDrawer.value = true
}
const onEditSlotClicked = (row: Row) => {
  selectedRow.value = row
  showSlotFormDrawer.value = true
}

const onDeleteSlotClicked = (row: Row) => {
  selectedRow.value = row
}

onMounted(() => {
})

onUnmounted(() => {
})
</script>

<style lang="scss">
@use '@/assets/styles/variables';
.slot-grid {
  &__item {
    padding: variables.$space-m;
  }
}
</style>
