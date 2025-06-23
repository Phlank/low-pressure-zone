import streamApi, { type ConnectionInformationResponse } from '@/api/resources/streamApi.ts'
import { ref, type Ref } from 'vue'
import { err, ok, type Result } from '@/types/result.ts'
import { defineStore } from 'pinia'

export const useConnectionInfoStore = defineStore('connectionInfoStore', () => {
  const liveInfoRef: Ref<ConnectionInformationResponse | undefined> = ref(undefined)
  const testInfoRef: Ref<ConnectionInformationResponse | undefined> = ref(undefined)

  let loadConnectionInfoPromise: Promise<Result<boolean, string>> | undefined = undefined
  const load = async () => {
    if (loadConnectionInfoPromise !== undefined) {
      return await loadConnectionInfoPromise
    }
    loadConnectionInfoPromise = loadConnectionInfo()
    const result = await loadConnectionInfoPromise
    loadConnectionInfoPromise = undefined
    return result
  }

  const loadConnectionInfo = async () => {
    const response = await streamApi.getConnectionInformation()
    if (!response.isSuccess()) {
      return err<boolean, string>('Failed to load connection information')
    }
    liveInfoRef.value = response.data().find((info) => info.streamType === 'Live')
    testInfoRef.value = response.data().find((info) => info.streamType === 'Test')
    return ok<boolean, string>(true)
  }

  const loadIfNotInitialized = async () => {
    if (liveInfoRef.value === undefined) return await load()
    return ok<boolean, string>(true)
  }

  const liveInfo = () => liveInfoRef.value
  const testInfo = () => testInfoRef.value

  return {
    load,
    loadIfNotInitialized,
    liveInfo,
    testInfo
  }
})
