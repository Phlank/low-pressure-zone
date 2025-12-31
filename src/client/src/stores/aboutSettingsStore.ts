import { defineStore } from 'pinia'
import { type Ref, ref } from 'vue'
import delay from '@/utils/delay.ts'
import settingsApi, { type AboutSettingsRequest } from '@/api/resources/settingsApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import { useToast } from 'primevue'
import { err, ok, type Result } from '@/types/result.ts'

export const useAboutSettingsStore = defineStore('aboutSettingsStore', () => {
  const topText = ref('')
  const bottomText = ref('')
  const autoRefreshing = ref<boolean>(false)
  const toast = useToast()
  const isLoading = ref(true)

  const refresh = async () => {
    const response = await settingsApi.getAboutSettings()
    if (tryHandleUnsuccessfulResponse(response, toast)) {
      return
    }
    topText.value = response.data().topText
    bottomText.value = response.data().bottomText
  }
  const autoRefresh = async () => {
    if (autoRefreshing.value) return
    autoRefreshing.value = true
    while (autoRefreshing.value) {
      await delay(300000)
      await refresh()
    }
  }
  refresh().then(() => {
    isLoading.value = false
    autoRefresh().then(() => {})
  })

  const update = async (
    formState: Ref<AboutSettingsRequest>,
    validation: FormValidation<AboutSettingsRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await settingsApi.putAboutSettings(formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    topText.value = formState.value.topText
    bottomText.value = formState.value.bottomText
    return ok()
  }

  return { topText, bottomText, update }
})
