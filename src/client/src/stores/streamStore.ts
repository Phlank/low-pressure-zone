import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'
import streamApi, {
  defaultStreamStatus,
  type StreamStatusResponse
} from '@/api/resources/streamApi.ts'
import { useRefresh } from '@/composables/useRefresh.ts'

export const useStreamStore = defineStore('streamStore', () => {
  const status = ref<StreamStatusResponse>(defaultStreamStatus)
  const getStatus = computed(() => status.value)

  const { isAutoRefreshing } = useRefresh(streamApi.getStatus, (data) => updateStatus(data), {
    autoRefreshInterval: 5000
  })

  const updateStatus = (newStatus: StreamStatusResponse) => {
    if (
      newStatus.isLive !== status.value?.isLive ||
      newStatus.isOnline !== status.value?.isOnline ||
      (newStatus.name ?? 'Unknown') !== status.value?.name ||
      newStatus.listenUrl !== status.value?.listenUrl ||
      newStatus.isStatusStale !== status.value?.isStatusStale ||
      newStatus.type !== status.value?.type ||
      newStatus.listenerCount !== status.value?.listenerCount
    ) {
      status.value = newStatus
    }
  }

  let isTitleUpdatingStarted = false
  const startTitleUpdating = () => {
    if (isTitleUpdatingStarted) return

    isTitleUpdatingStarted = true
    updateSiteTitle(getStatus.value)
  }

  watch(getStatus, (newStatus) => {
    updateSiteTitle(newStatus)
  })

  const updateSiteTitle = (newStatus: StreamStatusResponse) => {
    if (!isTitleUpdatingStarted) return

    const nameText = newStatus.name ?? 'Unknown'
    const liveText = newStatus.isLive ? 'Live' : 'Offline'
    document.title = `${nameText} - ${liveText} - Low Pressure Zone`
  }

  const stopTitleUpdating = () => {
    isTitleUpdatingStarted = false
    document.title = 'Low Pressure Zone'
  }

  return {
    status: getStatus,
    isAutoRefreshing,
    startTitleUpdating,
    stopTitleUpdating
  }
})
