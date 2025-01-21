<template>
  <main>
    <Panel :header="currentSchedule.audience" style="border-style: none">
      <DataTable :value="currentSchedule.timeslots">
        <Column header="Time">
          <template #body="slotProps">
            {{ slotProps.data.startTime.toLocaleTimeString() }}
          </template>
        </Column>
        <Column field="performance.name" header="Performer" />
      </DataTable>
      <!-- <Toolbar style="border-style: none">
        <template #center></template>
      </Toolbar> -->
    </Panel>
  </main>
</template>

<script setup lang="ts">
import { PerformanceType } from '@/models/performance/performanceType'
import type { Schedule } from '@/models/schedule/schedule'
import { Panel, DataTable, Column } from 'primevue'
import { ref, type Ref } from 'vue'

const today = new Date()
const currentSchedule: Ref<Schedule> = ref({
  audience: '/r/realdubstep',
  startDate: new Date(),
  description: "Hot take: Mere Sher isn't that good.",
  timeslots: [
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 16),
      performance: {
        name: 'Phlank',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 17),
      performance: {
        name: 'strgrll',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 18),
      performance: {
        name: 'Dubski',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 19),
      performance: {
        name: 'Phlank',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 20),
      performance: {
        name: 'Phlank',
        type: PerformanceType.Live
      }
    }
  ]
})
</script>

<style lang="scss" scoped>
@use '@/assets/variables.scss';
</style>
