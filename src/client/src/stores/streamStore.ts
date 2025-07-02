import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'
import streamApi, {
  defaultStreamStatus,
  type StreamStatusResponse
} from '@/api/resources/streamApi.ts'
import delay from '@/utils/delay.ts'

export const useStreamStore = defineStore('streamStore', () => {
  const statusRef = ref<StreamStatusResponse>(defaultStreamStatus)
  const status = computed(() => statusRef.value)

  let isStarted = false
  const startPolling = () => {
    if (isStarted) return
    isStarted = true
    pollStream().then(() => {})
  }

  const pollStream = async () => {
    while (isStarted) {
      try {
        const response = await streamApi.getStatus()
        if (response.isSuccess()) {
          updateStatus(response?.data())
        }
      } catch (error) {
        console.error(error)
      } finally {
        await delay(5000)
      }
    }
  }

  const stopPolling = () => {
    if (isStarted) {
      isStarted = false
    }
  }

  let isTitleUpdatingStarted = false
  const startTitleUpdating = () => {
    if (isTitleUpdatingStarted) return

    isTitleUpdatingStarted = true
    updateSiteTitle(status.value)
  }

  watch(status, (newStatus) => {
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

  const updateStatus = (newStatus: StreamStatusResponse) => {
    if (
      newStatus.isLive !== statusRef.value?.isLive ||
      newStatus.isOnline !== statusRef.value?.isOnline ||
      (newStatus.name ?? 'Unknown') !== statusRef.value?.name ||
      newStatus.listenUrl !== statusRef.value?.listenUrl ||
      newStatus.isStatusStale !== statusRef.value?.isStatusStale ||
      newStatus.type !== statusRef.value?.type ||
      newStatus.listenerCount !== statusRef.value?.listenerCount
    ) {
      statusRef.value = newStatus
    }
  }

  return {
    status,
    startPolling,
    stopPolling,
    startTitleUpdating,
    stopTitleUpdating
  }
})
