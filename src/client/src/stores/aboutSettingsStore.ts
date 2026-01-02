import { defineStore } from 'pinia'
import { ref } from 'vue'
import settingsApi from '@/api/resources/settingsApi.ts'
import { useToast } from 'primevue'
import { useRefresh } from '@/composables/useRefresh.ts'
import { useUpdateSettingFn } from '@/utils/storeFunctions.ts'
import { showEditSuccessToast } from '@/utils/toastUtils.ts'

export const useAboutSettingsStore = defineStore('aboutSettingsStore', () => {
  const topText = ref('')
  const bottomText = ref('')
  const toast = useToast()

  const { isLoading } = useRefresh(settingsApi.getAboutSettings, (data) => {
    topText.value = data.topText
    bottomText.value = data.bottomText
  })

  const update = useUpdateSettingFn(settingsApi.putAboutSettings, (settings) => {
    topText.value = settings.topText
    bottomText.value = settings.bottomText
    showEditSuccessToast(toast, 'About Settings')
  })

  return { isLoading, topText, bottomText, update }
})
