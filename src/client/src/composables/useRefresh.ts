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
  const isAutoRefreshing = ref(options?.useAutoRefresh ?? DEFAULT_USE_AUTO_REFRESH)
  const isLoading = ref(true)

  const autoRefresh = async () => {
    // noinspection InfiniteLoopJS
    while (isAutoRefreshing.value) {
      await delay(options?.autoRefreshInterval ?? DEFAULT_AUTO_REFRESH_INTERVAL)
      if (isAutoRefreshing.value) await refresh()
    }
  }

  const refresh = async () => {
    let response: ApiResponse<void, TResponse>
    if (apiFn.length === 0) response = await apiFn()
    else response = await apiFn(options?.params)
    if (!response.isSuccess()) return
    onRefresh(response.data())
  }

  watch(() => (options?.permissionFn ?? DEFAULT_PERMISSION_FN)(), (newVal) => {
    if (newVal) {
      refresh().then(() => {
        isLoading.value = false
        if (options?.useAutoRefresh) {
          isAutoRefreshing.value = true
          autoRefresh().then(() => {})
        }
      })
    } else {
      isLoading.value = false
      isAutoRefreshing.value = false
    }
  }, { immediate: true })

  const getIsLoading = computed(() => isLoading.value)
  return { isLoading: getIsLoading, refresh, isAutoRefreshing }
}
