import broadcastsApi, { type BroadcastResponse } from '@/api/resources/broadcastsApi.ts'
import { computed, type ComputedRef, type Ref, ref } from 'vue'
import { defineStore } from 'pinia'
import { err, ok, type Result } from '@/types/result.ts'

export const useBroadcastStore = defineStore('broadcastStore', () => {
  const loadedBroadcasts: Ref<BroadcastResponse[] | undefined> = ref(undefined)
  const broadcasts: ComputedRef<BroadcastResponse[]> = computed(() => loadedBroadcasts.value ?? [])

  let loadBroadcastsPromise: Promise<Result<BroadcastResponse[], string>> | undefined = undefined
  const loadBroadcasts = async () => {
    const response = await broadcastsApi.get()
    if (!response.isSuccess()) {
      return err<BroadcastResponse[], string>('Failed to load broadcasts')
    }
    loadedBroadcasts.value = response.data()
    return ok<BroadcastResponse[], string>(response.data())
  }

  const load = async () => {
    loadBroadcastsPromise ??= loadBroadcasts()
    return await loadBroadcastsPromise
  }

  const loadIfNotInitialized = async () => {
    if (loadedBroadcasts.value === undefined) {
      return load()
    }
    return ok<BroadcastResponse[], string>(loadedBroadcasts.value)
  }

  return {
    load,
    loadIfNotInitialized,
    broadcasts
  }
})
