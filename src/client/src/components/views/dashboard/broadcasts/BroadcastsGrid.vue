<template>
  <div class="broadcasts-grid">
    <div v-if="!isMobile && !broadcasts.isLoading">
      <DataTable
        :rows="10"
        :value="broadcasts.broadcasts"
        data-key="broadcastId"
        paginator>
        <Column
          field="start"
          header="Start">
          <template #body="{ data }: { data: BroadcastResponse }">
            {{ parseDate(data.start).toLocaleDateString() }} <br />
            {{ formatReadableTime(parseDate(data.start)) }}
          </template>
        </Column>
        <Column header="Duration">
          <template #body="{ data }: { data: BroadcastResponse }">
            <div v-if="data.end">
              {{ formatDurationTimestamp(getDuration(data.start, data.end!)) }}
            </div>
          </template>
        </Column>
        <Column
          v-if="showStreamerName"
          field="streamerDisplayName"
          header="Streamer" />
        <Column class="grid-action-col grid-action-col--2">
          <template #body="{ data }: { data: BroadcastResponse }">
            <GridActions
              :show-disconnect="data.isDisconnectable"
              :show-download="data.isDownloadable"
              @disconnect="handleDisconnectAction"
              @download="handleDownloadClicked(data)" />
          </template>
        </Column>
      </DataTable>
    </div>
    <DataView
      v-if="isMobile && !broadcasts.isLoading"
      :paginator="broadcasts.broadcasts.length > 5"
      :paginator-template="mobilePaginatorTemplate"
      :rows="5"
      :value="broadcasts.broadcasts"
      data-key="broadcastId">
      <template #empty>
        <ListItem>
          <template #left>{{
            broadcasts.isLoading ? 'Loading...' : 'No items to display'
          }}</template>
        </ListItem>
      </template>
      <template #list="{ items }: { items: BroadcastResponse[] }">
        <div
          v-for="(broadcast, index) in items"
          :key="broadcast.broadcastId">
          <ListItem>
            <template #left>
              <div class="broadcasts-grid__mobile-time-info">
                <span>{{ formatMobileTimeInfo(broadcast) }}</span>
              </div>
              <div>
                {{ broadcast.streamerDisplayName ?? 'Unknown' }}
              </div>
            </template>
            <template #right>
              <GridActions
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
    <Skeleton
      v-if="broadcasts.isLoading"
      style="height: 300px" />
    <Dialog
      v-model:visible="showDisconnectDialog"
      header="Disconnect Streamer"
      modal
      :draggable="false">
      <p>
        Most DJ software will automatically try to reconnect when disconnected. So, the DJ's
        streaming account must be disabled for a time. How long should that be?
      </p>
      <template #footer>
        <Button outlined label="None" @click="handleDisconnect()" />
        <Button outlined label="5 Minutes" @click="handleDisconnect(5)" />
        <Button outlined label="1 Hour" @click="handleDisconnect(60)" />
        <Button outlined label="24 Hours" @click="handleDisconnect(1440)" />
        <Button outlined label="Indefinitely" @click="handleDisconnect(0)" />
      </template>
    </Dialog>
  </div>
</template>

<script lang="ts" setup>
import { inject, ref, type Ref } from 'vue'
import { useBroadcastStore } from '@/stores/broadcastStore.ts'
import { Column, DataTable, DataView, Divider, Skeleton, useToast, Dialog, Button } from 'primevue'
import broadcastsApi, { type BroadcastResponse } from '@/api/resources/broadcastsApi.ts'
import {
  formatDurationTimestamp,
  formatReadableTime,
  getDuration,
  parseDate
} from '@/utils/dateUtils.ts'
import GridActions from '@/components/data/grid-actions/GridActions.vue'
import { mobilePaginatorTemplate } from '@/constants/componentTemplates.ts'
import ListItem from '@/components/data/ListItem.vue'

const toast = useToast()
const broadcasts = useBroadcastStore()
const isMobile: Ref<boolean> | undefined = inject('isMobile')

defineProps<{
  showStreamerName?: boolean
}>()

const formatMobileTimeInfo = (broadcast: BroadcastResponse) => {
  const date = parseDate(broadcast.start).toLocaleDateString()
  const time = formatReadableTime(parseDate(broadcast.start))
  const duration =
    broadcast.end === null
      ? 'Live'
      : formatDurationTimestamp(getDuration(broadcast.start, broadcast.end))
  return `${date} ${time} | ${duration}`
}

const handleDownloadClicked = (broadcast: BroadcastResponse) => {
  broadcastsApi.download(broadcast.streamerId ?? 0, broadcast.broadcastId)
}

const showDisconnectDialog = ref(false)
const handleDisconnectAction = () => {
  showDisconnectDialog.value = true
}

const handleDisconnect = async (disableMinutes?: number) => {
  const result = await broadcasts.disconnectAsync(disableMinutes)
  if (!result.isSuccess) return
  toast.add({
    severity: 'success',
    summary: 'Disconnected',
    detail: 'Streamer disconnected successfully.'
  })
  showDisconnectDialog.value = false
}
</script>

<style lang="scss">
.broadcasts-grid {
  &__mobile-time-info {
    font-size: small;
  }
}
</style>
