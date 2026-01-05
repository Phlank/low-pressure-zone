import type { InviteResponse } from '@/api/resources/invitesApi.ts'
import invitesApi from '@/api/resources/invitesApi.ts'
import { computed, ref, type Ref } from 'vue'
import { defineStore } from 'pinia'
import { useToast } from 'primevue'
import { err, ok } from '@/types/result.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { getEntity } from '@/utils/arrayUtils.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import { useCreatePersistentItemFn } from '@/utils/storeFns.ts'

export const useInviteStore = defineStore('inviteStore', () => {
  const items: Ref<InviteResponse[]> = ref([])

  useRefresh(invitesApi.get, (data) => {
    items.value = data
  })

  const toast = useToast()
  const create = useCreatePersistentItemFn(invitesApi.post, (id, request) => {
    const item: InviteResponse = {
      id: id,
      communityIds: [request.communityId],
      invitedAt: new Date().toUTCString(),
      lastSentAt: new Date().toUTCString(),
      displayName: request.email,
      email: request.email
    }
    items.value.unshift(item)
    showSuccessToast(toast, 'Created', 'Invite created successfully.', request.email)
  })

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
