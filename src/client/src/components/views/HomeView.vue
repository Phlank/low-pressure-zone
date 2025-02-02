<template>
  <div class="flex-variable-space-between">
    <Panel
      :header="currentSchedule.audience"
      class="flex-variable-space-between__left flex-variable-space-between__left--variable-height"
    >
      <p>{{ currentSchedule.description }}</p>
      <DataTable :value="currentSchedule.timeslots">
        <Column header="Time">
          <template #body="slotProps">
            {{ slotProps.data.startTime.toLocaleTimeString() }}
          </template>
        </Column>
        <Column field="performance.name" header="Performer" />
      </DataTable>
    </Panel>
    <iframe
      src="https://discord.com/widget?id=722643085137412096&theme=dark"
      width="350"
      height="500"
      allowtransparency="true"
      frameborder="0"
      sandbox="allow-popups allow-popups-to-escape-sandbox allow-same-origin allow-scripts"
      class="discord-iframe flex-variable-space-between__right"
    ></iframe>
  </div>
</template>

<script setup lang="ts">
import { Panel, DataTable, Column } from 'primevue'
import { ref, type Ref } from 'vue'

const today = new Date()
const currentSchedule: Ref<Schedule> = ref({
  audience: '/r/realdubstep',
  startDate: new Date(),
  description: "Hot take: Mere Sher isn't that good.",
  timeslots: [
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 18),
      performance: {
        name: 'Phlank',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 19),
      performance: {
        name: 'strgrll',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 20),
      performance: {
        name: 'dubski',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 21),
      performance: {
        name: 'Sovereign',
        type: PerformanceType.Live
      }
    },
    {
      startTime: new Date(today.getFullYear(), today.getMonth() - 1, today.getDate(), 22),
      performance: {
        name: 'repulsion',
        type: PerformanceType.Live
      }
    }
  ]
})
</script>

<style lang="scss" scoped>
@use '@/assets/variables.scss';

.discord-iframe {
  border-radius: variables.$space-m;
}
</style>
