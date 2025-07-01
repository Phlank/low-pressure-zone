import { computed, ref } from 'vue'
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
  const start = () => {
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

  const stop = () => {
    if (isStarted) {
      isStarted = false
    }
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
    start,
    stop
  }
})
