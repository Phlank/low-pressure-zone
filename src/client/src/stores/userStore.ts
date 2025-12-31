import { defineStore } from 'pinia'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { computed, ref, type Ref } from 'vue'
import { getEntity } from '@/utils/arrayUtils.ts'
import { useToast } from 'primevue'
import delay from '@/utils/delay.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'
import { err, ok, type Result } from '@/types/result.ts'

export const useUserStore = defineStore('userStore', () => {
  const users: Ref<UserResponse[]> = ref([])
  const isLoading = ref(true)
  const toast = useToast()

  let autoRefreshing = false
  const autoRefresh = async () => {
    if (autoRefreshing) return
    autoRefreshing = true
    while (autoRefreshing) {
      await delay(300000)
      await refresh()
    }
  }
  const refresh = async () => {
    const response = await usersApi.get()
    if (tryHandleUnsuccessfulResponse(response, toast)) return
    users.value = response.data()
  }
  const auth = useAuthStore()
  if (auth.isInRole(roles.admin)) {
    refresh().then(() => {
      isLoading.value = false
      autoRefresh().then(() => {})
    })
  }

  const getUser = (id: string) => getEntity(users.value, id)

  const makeStreamer = async (id: string): Promise<Result> => {
    const entity = getEntity(users.value, id)
    if (!entity || entity.isStreamer) return err()
    const response = await usersApi.createStreamer(id)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    entity.isStreamer = true
    return ok()
  }

  const getItems = computed(() => users.value)

  return { items: getItems, getUser, makeStreamer }
})
