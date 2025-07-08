<template>
  <div class="broadcasts-grid">
    <div v-if="!isMobile">
      <DataTable
        :value="broadcastStore.broadcasts"
        data-key="broadcastId">
        <Column
          field="start"
          header="Start">
          <template #body="{ data }: { data: BroadcastResponse }">
            {{ parseDate(data.start).toLocaleDateString() }} <br />
            {{ formatTimeslot(parseDate(data.start)) }}
          </template>
        </Column>
        <Column header="Duration">
          <template #body="{ data }: { data: BroadcastResponse }">
            <div v-if="data.end">
              {{ parseDate(data.end).getTime() - parseDate(data.start).getTime() }}
            </div>
          </template>
        </Column>
        <Column
          field="broadcasterDisplayName"
          header="AzuraCaster" />
        <Column
          field="nearestPerformerName"
          header="Performer (based on timeslots)" />
        <Column
          field="isDownloadable"
          header="Downloadable">
          <template #body="{ data }: { data: BroadcastResponse }">
            {{ data.isDownloadable ? 'Yes' : '' }}
          </template>
        </Column>
      </DataTable>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { inject, type Ref } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import { Column, DataTable } from 'primevue'
import type { BroadcastResponse } from '@/api/resources/broadcastsApi.ts'
import { formatTimeslot, parseDate } from '@/utils/dateUtils.ts'

const broadcastStore = useBroadcastStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')
</script>
