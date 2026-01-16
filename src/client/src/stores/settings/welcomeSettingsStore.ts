import settingsApi, { type TabContent } from '@/api/resources/settingsApi.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import { ref, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { useAuthStore } from '@/stores/authStore.ts'
import { useUpdateSettingFn } from '@/utils/storeFns.ts'
import { useToast } from 'primevue'
import { showEditSuccessToast } from '@/utils/toastUtils.ts'

export const useWelcomeSettingsStore = defineStore('welcomeSettingsStore', () => {
  const tabs: Ref<TabContent[]> = ref([])
  const auth = useAuthStore()
  const toast = useToast()

  const { isLoading } = useRefresh(
    settingsApi.getWelcomeSettings,
    (data) => {
      tabs.value = data.tabs
    },
    { permissionFn: () => auth.isLoggedIn }
  )

  const update = useUpdateSettingFn(settingsApi.putWelcomeSettings, (settings) => {
    tabs.value = settings.tabs
    showEditSuccessToast(toast, 'Welcome Settings')
  })

  return { tabs, isLoading, update }
})
