import { computed, ref, watch } from 'vue'
import { defineStore } from 'pinia'
import streamApi, {
  defaultStreamStatus,
  type Mount,
  type StreamStatusResponse
} from '@/api/resources/streamApi.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import areObjectPropertiesEqual from '@/utils/areObjectPropertiesEqual.ts'
import { useLocalStorage } from '@vueuse/core'

export const useStreamStore = defineStore('streamStore', () => {
  const status = ref<StreamStatusResponse>(defaultStreamStatus)
  const getStatus = computed(() => status.value)
  const mounts = computed(() =>
    status.value ? status.value.mounts.sort((a, b) => a.name.localeCompare(b.name)) : []
  )
  const selectedMountName = useLocalStorage<string | undefined>('selectedMount', undefined)
  const selectedMountUrl = computed(() => mounts.value.find(mount => selectedMountName.value === mount.name)?.url)

  const { isAutoRefreshing } = useRefresh(streamApi.getStatus, (data) => updateStatus(data), {
    autoRefreshInterval: 5000
  })

  const updateStatus = (newStatus: StreamStatusResponse) => {
    if (areObjectPropertiesEqual(status.value, newStatus)) return
    status.value = newStatus
    if (selectedMountName.value === undefined) {
      selectedMountName.value = newStatus.mounts.find((mount) => mount.name === '320')?.name
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

  const selectMount = (mount: Mount | undefined) => {
    if (mount) selectedMountName.value = mount.name
  }

  return {
    status: getStatus,
    mounts,
    selectedMountName,
    selectedMountUrl,
    selectMount,
    isAutoRefreshing,
    startTitleUpdating,
    stopTitleUpdating
  }
})
