import type { InviteRequest, InviteResponse } from '@/api/resources/invitesApi.ts'
import invitesApi from '@/api/resources/invitesApi.ts'
import { computed, ref, type Ref } from 'vue'
import { defineStore } from 'pinia'
import delay from '@/utils/delay.ts'
import { useToast } from 'primevue'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import { err, ok, type Result } from '@/types/result.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { getEntity } from '@/utils/arrayUtils.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'

export const useInviteStore = defineStore('inviteStore', () => {
  const items: Ref<InviteResponse[]> = ref([])
  const isLoading = ref(true)

  const autoRefreshing = ref(false)
  const autoRefresh = async () => {
    if (autoRefreshing.value) return
    autoRefreshing.value = true
    while (autoRefreshing.value) {
      await delay(300000)
      await refresh()
    }
  }
  const refresh = async () => {
    const response = await invitesApi.get()
    if (!response.isSuccess()) return
    items.value = response.data()
  }

  const auth = useAuthStore()
  if (auth.isInRole(roles.admin))
    refresh().then(() => {
      isLoading.value = false
      autoRefresh().then(() => {})
    })

  const toast = useToast()
  const create = async (
    formState: Ref<InviteRequest>,
    validation: FormValidation<InviteRequest>
  ): Promise<Result> => {
    if (!validation.validate()) return err()
    const response = await invitesApi.post(formState.value)
    if (tryHandleUnsuccessfulResponse(response, toast, validation)) return err()
    const item: InviteResponse = {
      id: response.getCreatedId(),
      invitedAt: new Date().toUTCString(),
      lastSentAt: new Date().toUTCString(),
      displayName: formState.value.email,
      ...formState.value
    }
    items.value.unshift(item)
    showSuccessToast(toast, 'Created', 'Invite created successfully.', formState.value.email)
    return ok()
  }

  const resendEmail = async (id: string) => {
    const entity = getEntity(items.value, id)
    if (entity === undefined) return err()
    const response = await invitesApi.postResend(id)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    entity.lastSentAt = new Date().toUTCString()
    return ok()
  }

  const requestEmail = async (email: string) => {
    const response = await invitesApi.getResend(email)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    return ok()
  }

  const getItems = computed(() => items.value)

  return { items: getItems, create, resendEmail, requestEmail }
})
