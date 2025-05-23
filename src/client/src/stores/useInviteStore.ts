import type { InviteResponse } from '@/api/resources/invitesApi.ts'
import invitesApi from '@/api/resources/invitesApi.ts'
import { computed, ref, type Ref } from 'vue'
import { defineStore } from 'pinia'

export const useInviteStore = defineStore('inviteStore', () => {
  const loadedInvites: Ref<InviteResponse[]> = ref([])

  let loadInvitesPromise: Promise<void> | undefined = undefined
  const loadInvites = async () => {
    const response = await invitesApi.get()
    if (!response.isSuccess()) return
    loadedInvites.value = response.data()
  }

  const loadInvitesAsync = async () => {
    loadInvitesPromise ??= loadInvites()
    await loadInvitesPromise
    loadInvitesPromise = undefined
  }

  const invites = computed(() => loadedInvites.value)

  return { loadInvitesAsync, invites }
})
