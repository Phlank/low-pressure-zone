import { computed, ref, watch } from 'vue'
import delay from '@/utils/delay.ts'
import type { ApiResponse } from '@/api/apiResponse.ts'

const DEFAULT_AUTO_REFRESH_INTERVAL = 300000 // 5 minutes
const DEFAULT_USE_AUTO_REFRESH = true
const DEFAULT_PERMISSION_FN = () => true

interface UseRefreshOptions<TParams = void> {
  /**
   * Whether to enable automatic refreshing of data.
   *
   * Default: `true`
   */
  useAutoRefresh?: boolean
  /**
   * The interval (in milliseconds) at which to automatically refresh the data.
   *
   * Default: `300000` (5 minutes)
   */
  autoRefreshInterval?: number
  /**
   * Check user permissions before pulling data.
   */
  permissionFn?: () => boolean
  /**
   * Parameters to pass to the API function.
   */
  params?: TParams
}

export const useRefresh = <TResponse, TParams = void>(
  apiFn: (arg?: TParams) => Promise<ApiResponse<void, TResponse>>,
  onRefresh: (data: TResponse) => void,
  options?: UseRefreshOptions<TParams>
) => {
  const isAutoRefreshing = ref(false)
  const isLoading = ref(true)

  let autoRefreshController: AbortController | null = null
  const autoRefresh = async () => {
    autoRefreshController = new AbortController()
    while (isAutoRefreshing.value) {
      await Promise.all([
        delay(options?.autoRefreshInterval ?? DEFAULT_AUTO_REFRESH_INTERVAL, autoRefreshController),
        refresh()
      ])
    }
  }

  const stopAutoRefresh = () => {
    if (isAutoRefreshing.value) {
      isAutoRefreshing.value = false
    }

    if (autoRefreshController !== null && !autoRefreshController.signal.aborted) {
      autoRefreshController.abort()
      autoRefreshController = null
    }
  }

  const startAutoRefresh = () => {
    if (!isAutoRefreshing.value) {
      isAutoRefreshing.value = true
      autoRefresh().then().catch(() => {
        isAutoRefreshing.value = false
        startAutoRefresh()
      })
    }
  }

  const refresh = async () => {
    let response: ApiResponse<void, TResponse>
    if (apiFn.length === 0) response = await apiFn()
    else response = await apiFn(options?.params)
    if (!response.isSuccess()) return
    onRefresh(response.data())
  }

  watch(
    () => (options?.permissionFn ?? DEFAULT_PERMISSION_FN)(),
    async (newVal) => {
      if (newVal) {
        isLoading.value = false
        if (options?.useAutoRefresh ?? DEFAULT_USE_AUTO_REFRESH) {
          startAutoRefresh()
        }
      } else {
        isLoading.value = false
      }
    },
    { immediate: true }
  )

  const getIsLoading = computed(() => isLoading.value)
  return { isLoading: getIsLoading, refresh, isAutoRefreshing, startAutoRefresh, stopAutoRefresh }
}
