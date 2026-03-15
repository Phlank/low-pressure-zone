import { defineStore } from 'pinia'
import { computed, ref } from 'vue'
import { useToast } from 'primevue'
import { useRefresh } from '@/composables/useRefresh.ts'
import settingsApi from '@/api/resources/settingsApi.ts'
import { useUpdateSettingFn } from '@/utils/storeFns.ts'
import { showEditSuccessToast } from '@/utils/toastUtils.ts'

export const usePrivacyPolicySettingsStore = defineStore('privacyPolicySettingsStore', () => {
  const content = ref('')
  const toast = useToast()

  const { isLoading } = useRefresh(
    settingsApi.getPrivacyPolicySettings,
    (data) => {
      content.value = data.privacyPolicy
    },
    {
      useAutoRefresh: false
    }
  )

  const update = useUpdateSettingFn(settingsApi.putPrivacyPolicySettings, (settings) => {
    content.value = settings.privacyPolicy
    showEditSuccessToast(toast, 'Privacy Policy Settings')
  })

  const contentValue = computed(() => content.value)
  return { isLoading, privacyPolicy: contentValue, update }
})
