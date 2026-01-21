import broadcastsApi, { type BroadcastResponse } from '@/api/resources/broadcastsApi.ts'
import { computed, type ComputedRef, type Ref, ref } from 'vue'
import { defineStore } from 'pinia'
import { useRefresh } from '@/composables/useRefresh.ts'
import {useToast} from "primevue";
import tryHandleUnsuccessfulResponse from "@/api/tryHandleUnsuccessfulResponse.ts";
import { err, ok, type Result } from '@/types/result.ts'

export const useBroadcastStore = defineStore('broadcastStore', () => {
  const loadedBroadcasts: Ref<BroadcastResponse[]> = ref([])
  const broadcasts: ComputedRef<BroadcastResponse[]> = computed(() => loadedBroadcasts.value)
  const toast = useToast()

  const { refresh, isLoading } = useRefresh(broadcastsApi.get, (data) => {
    loadedBroadcasts.value = data
  })

  const disconnectAsync = async (disableMinutes?: number): Promise<Result> => {
    const response = await broadcastsApi.disconnect({ disableMinutes: disableMinutes })
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    if (loadedBroadcasts.value[0])
      loadedBroadcasts.value[0].isDisconnectable = false
    return ok()
  }

  return {
    refresh,
    isLoading,
    broadcasts,
    disconnectAsync
  }
})
