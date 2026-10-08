<template>
  <div class="home-schedule-grid">
    <DataTable
      :value="rows"
      :loading="schedules.isLoading">
      <template #empty> No DJs have signed up for this schedule yet. </template>
      <Column header="Time">
        <template #body="{ data }: { data: Row }">
          <SlotTime
            v-if="data.start"
            :date="data.start" />
        </template>
      </Column>
      <Column
        field="slot"
        style="width: 100%">
        <template #body="{ data }: { data: Row }">
          <div v-if="data.slot?.type === 'Clash'">
            <div class="home-schedule-grid__slot--clash">
              <div class="home-schedule-grid__slot--clash__performers">
                <div class="home-schedule-grid__slot--clash__performers__name">
                  {{ performers.getById(data.slot.performerOneId)?.name }}
                </div>
                <Divider type="dotted"><i>vs.</i></Divider>
                <div class="home-schedule-grid__slot--clash__performers__name">
                  {{ performers.getById(data.slot.performerTwoId)?.name }}
                </div>
              </div>
              <Divider
                v-if="isMobile"
                type="dotted" />
              <div class="home-soundclash-grid__slot__rounds">
                <div
                  v-for="round in data.slot.rounds"
                  :key="round">
                  {{ round }}
                </div>
              </div>
            </div>
          </div>
          <div v-else-if="data.slot?.type === 'Hourly'">
            <SlotName
              :performer="performers.getById(data.slot.performerId)?.name || ''"
              :is-prerecorded="!data.slot.isPrerecorded"
              :name="data.slot.subtitle" />
          </div>
        </template>
      </Column>
    </DataTable>
  </div>
</template>

<script setup lang="ts">
import { DataTable, Column, Divider } from 'openvue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'
import { computed, inject, type Ref, ref, watch } from 'vue'
import type { SlotResponse } from '@/api/resources/schedulesApi.ts'
import { isDateInSlot, parseDate, timesBetween } from '@/utils/dateUtils.ts'
import SlotTime from '@/components/controls/SlotTime.vue'
import { usePerformerStore } from '@/stores/performerStore.ts'
import SlotName from '@/components/controls/SlotName.vue'

const schedules = useScheduleStore()
const performers = usePerformerStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')

const props = defineProps<{ scheduleId: string }>()
const schedule = computed(() => schedules.getScheduleById(props.scheduleId))

interface Row {
  start: Date
  slot?: SlotResponse
}
const rows: Ref<Row[]> = ref([])
const setupRows = () => {
  const newRows: Row[] = []
  if (!schedule.value || schedule.value.slots.length === 0) {
    rows.value = newRows
    return
  }
  const slots = schedule.value.slots
  const startFirst = new Date(slots[0]!.startsAt)
  const endLast = new Date(slots.at(-1)!.startsAt)
  const hours = timesBetween(startFirst, endLast, 60)

  for (const hour of hours) {
    const slot = slots.find((s) => isDateInSlot(hour, s))

    // Middle of a clash, we don't use multiple rows for clashes, skip iteration
    if (slot?.type === 'Clash') {
      if (parseDate(slot.startsAt) !== hour) continue
    }

    newRows.push({
      start: hour,
      slot: slot
    })
  }

  rows.value = newRows
}

watch(
  () => schedule.value,
  () => setupRows(),
  { immediate: true, deep: true }
)
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.home-schedule-grid {
  &__slot {
    display: flex;
    justify-content: space-between;
    align-items: center;

    @include variables.mobile() {
      flex-direction: column;
    }

    &__performers {
      display: flex;
      flex-direction: column;
      align-items: center;
      width: 100%;
      margin: variables.$space-s 0;
      text-align: center;
      &__name {
        font-weight: bolder;
        font-size: large;
      }
    }

    &__rounds {
      width: 100%;
      display: flex;
      flex-direction: column;
      align-items: center;
      text-align: center;
      gap: variables.$space-m;
      font-weight: 500;
    }

    .p-divider-content {
      z-index: 0;
    }
  }
}
</style>
