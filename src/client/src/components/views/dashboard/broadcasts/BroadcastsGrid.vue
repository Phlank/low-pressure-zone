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
              {{ formatDuration(getDuration(data.end, data.start)) }}
            </div>
          </template>
        </Column>
        <Column
          v-if="showStreamerName"
          field="streamerDisplayName"
          header="AzuraCaster" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: BroadcastResponse }">
            <GridActions
              :show-delete="data.isDeletable"
              :show-download="data.isDownloadable"
              @download="handleDownloadClicked(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <DataView
      v-if="isMobile"
      :paginator="broadcastStore.broadcasts.length > 5"
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="broadcastStore.broadcasts"
      data-key="broadcastId">
      <template #empty>
        <ListItem>
          <template #left>No items to display.</template>
        </ListItem>
      </template>
      <template #list="{ items }: { items: BroadcastResponse[] }">
        <div
          v-for="(broadcast, index) in items"
          :key="broadcast.broadcastId">
          <ListItem>
            <template #left>
              <div style="font-size: small">
                {{ formatMobileTimeInfo(broadcast) }}
              </div>
              <div>
                {{ broadcast.streamerDisplayName ?? 'Unknown' }}
              </div>
            </template>
            <template #right>
              <GridActions
                :show-delete="broadcast.isDeletable"
                :show-download="broadcast.isDownloadable"
                @download="handleDownloadClicked(broadcast)" />
            </template>
          </ListItem>
          <Divider v-if="index < items.length - 1" />
          <div
            v-else
            style="padding-bottom: 20px"></div>
        </div>
      </template>
    </DataView>
  </div>
</template>

<script lang="ts" setup>
import { inject, type Ref } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import { Column, DataTable, DataView, Divider } from 'primevue'
import broadcastsApi, { type BroadcastResponse } from '@/api/resources/broadcastsApi.ts'
import { formatDuration, formatTimeslot, getDuration, parseDate } from '@/utils/dateUtils.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import ListItem from '@/components/data/ListItem.vue'

const broadcastStore = useBroadcastStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')

defineProps<{
  showStreamerName?: boolean
}>()

const formatMobileTimeInfo = (broadcast: BroadcastResponse) => {
  let out = parseDate(broadcast.start).toLocaleString()
  if (broadcast.end) {
    out += ` - ${formatDuration(getDuration(broadcast.start, broadcast.end))}`
  }
  return out
}

const handleDownloadClicked = (broadcast: BroadcastResponse) => {
  broadcastsApi.download(broadcast.streamerId ?? 0, broadcast.broadcastId)
}
</script>
