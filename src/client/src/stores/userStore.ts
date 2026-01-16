import { defineStore } from 'pinia'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { computed, ref, type Ref } from 'vue'
import { getEntity, getEntityMap} from '@/utils/arrayUtils.ts'
import { useToast } from 'primevue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { err, ok, type Result } from '@/types/result.ts'
import { useRefresh } from '@/composables/useRefresh.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'

export const useUserStore = defineStore('userStore', () => {
  const users: Ref<UserResponse[]> = ref([])
  const userMap: Ref<Partial<Record<string, UserResponse>>> = ref({})
  const toast = useToast()
  const auth = useAuthStore()

  useRefresh(
    usersApi.get,
    (data) => {
      users.value = data
      userMap.value = getEntityMap(users.value)
    },
    { permissionFn: () => auth.isInAnyRoles(roles.admin, roles.organizer) }
  )

  const getUser = (id: string) => userMap.value[id]

  const makeStreamer = async (id: string): Promise<Result> => {
    const entity = getEntity(users.value, id)
    if (!entity || entity.isStreamer) return err()
    const response = await usersApi.createStreamer(id)
    if (tryHandleUnsuccessfulResponse(response, toast)) return err()
    entity.isStreamer = true
    return ok()
  }

  const getUsers = computed(() => users.value)

  return { users: getUsers, getUser, makeStreamer }
})
