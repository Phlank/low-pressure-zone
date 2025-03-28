import { defineStore } from 'pinia'
import usersApi, { type UserResponse } from '@/api/resources/usersApi.ts'
import { computed, ref, type Ref } from 'vue'

export const useUserStore = defineStore('userStore', () => {
  const loadedUsers: Ref<UserResponse[]> = ref([])
  const loadedUserMap: Ref<UserMap> = ref({})

  let loadUsersPromise: Promise<void> | undefined = undefined

  const loadUsers = async () => {
    const response = await usersApi.get()
    if (!response.isSuccess()) {
      console.log(JSON.stringify(response))
      return
    }
    loadedUsers.value = response.data!
    const userMap: UserMap = {}
    loadedUsers.value.forEach((user) => {
      userMap[user.id] = user
    })
    loadedUserMap.value = userMap
  }

  const loadUsersAsync = async () => {
    if (loadUsersPromise === undefined) {
      loadUsersPromise = loadUsers()
    }
    await loadUsersPromise
    loadUsersPromise = undefined
  }
  const getUser = (id: string) => {
    return loadedUserMap.value[id]
  }
  const users = computed(() => loadedUsers.value)

  return { users, getUser, loadUsersAsync }
})

type UserMap = Record<string, UserResponse>
