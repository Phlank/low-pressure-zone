import settingsApi, { type AboutSettingsRequest } from '@/api/resources/settingsApi.ts'
import { onMounted, type Ref, ref } from 'vue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { type ToastServiceMethods } from 'primevue'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import delay from '@/utils/delay.ts'

export default (toast: ToastServiceMethods) => {
  const topText = ref('')
  const bottomText = ref('')
  const autoRefreshing = ref<boolean>(false)

  onMounted(async () => {
    await refresh()
    autoRefresh().then(() => {})
  })

  const autoRefresh = async () => {
    if (autoRefreshing.value) return
    autoRefreshing.value = true
    while (autoRefreshing.value) {
      await delay(300000)
      await refresh()
    }
  }

  const refresh = async () => {
    const response = await settingsApi.getAboutSettings()
    if (tryHandleUnsuccessfulResponse(response, toast)) {
      return
    }
    topText.value = response.data().topText
    bottomText.value = response.data().bottomText
  }

  const update = async (
    formState: Ref<AboutSettingsRequest>,
    validation: FormValidation<AboutSettingsRequest>
  ) => {
    if (!validation.validate()) return
    const response = await settingsApi.putAboutSettings(formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return
    topText.value = formState.value.topText
    bottomText.value = formState.value.bottomText
  }

  return { topText, bottomText, update }
}
