import api from '@/api/api'
import type { UserResponse } from '@/api/users/userResponse'
import { defineStore } from 'pinia'
import { ref, type Ref } from 'vue'

export const useUserStore = defineStore('userStore', () => {
  const isLoggedInRef: Ref<boolean | undefined> = ref(undefined)
  const userResponse: Ref<UserResponse> = ref({
    id: '',
    email: '',
    username: '',
    roles: []
  })

  let loadUserInfoPromise: Promise<void> | undefined = undefined
  const load = async () => {
    if (loadUserInfoPromise !== undefined) {
      await loadUserInfoPromise
      return
    }
    loadUserInfoPromise = loadUserInfo()
    await loadUserInfoPromise
    loadUserInfoPromise = undefined
  }

  const loadUserInfo = async () => {
    const response = await api.users.info.get()
    if (response.status === 0) return
    isLoggedInRef.value = response.isSuccess()
    if (response.isSuccess()) {
      userResponse.value = response.data!
    }
  }

  const loadIfNotInitialized = async () => {
    if (isLoggedInRef.value === undefined) await load()
  }

  const isLoggedIn = () => isLoggedInRef.value ?? false

  const getId = () => userResponse.value.id

  const getEmail = () => userResponse.value.email

  const getUsername = () => userResponse.value.username

  const getRoles = () => userResponse.value.roles

  const isInAnySpecifiedRole = (...rolesToCheck: string[]): boolean => {
    if (rolesToCheck.length === 0) return true
    if (getRoles().length === 0) return false

    for (let i = 0; i < getRoles().length; i++) {
      if (rolesToCheck.some((roleToCheck) => roleToCheck === getRoles()[i])) {
        return true
      }
    }
    return false
  }

  const clear = () => {
    isLoggedInRef.value = false
    userResponse.value.id = ''
    userResponse.value.email = ''
    userResponse.value.username = ''
    userResponse.value.roles = []
  }

  return {
    load,
    loadIfNotInitialized,
    isLoggedIn,
    getId,
    getEmail,
    getUsername,
    getRoles,
    /**
     * @returns `true` if the user is in one of the specified roles or no roles were specified.
     */
    isInAnySpecifiedRole,
    clear
  }
})
