import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import type { UserResponse } from '@/api/users/userResponse'
import { defineStore } from 'pinia'
import { computed, ref, type Ref } from 'vue'

export const useUserStore = defineStore('userStore', () => {
  const isLoggedInRef: Ref<boolean | undefined> = ref(undefined)
  const userResponse: Ref<UserResponse | undefined> = ref(undefined)

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
    userResponse.value = response.data
  }

  const loadIfNotInitialized = async () => {
    if (isLoggedInRef.value === undefined) await load()
  }

  const isLoggedIn = async () => {
    await loadIfNotInitialized()
    if (isLoggedInRef.value === undefined) return false
    return isLoggedInRef.value
  }

  const getId = async () => {
    await loadIfNotInitialized()
    return userResponse.value?.id
  }

  const getEmail = async () => {
    await loadIfNotInitialized()
    return userResponse.value?.email
  }

  const getUsername = async () => {
    await loadIfNotInitialized()
    return userResponse.value?.username
  }

  const getRoles = async () => {
    await loadIfNotInitialized()
    return userResponse.value?.roles
  }

  const isInAnySpecifiedRole = async (...rolesToCheck: string[]): Promise<boolean> => {
    if (rolesToCheck.length == 0) return true
    const userRoles = await getRoles()
    if (userRoles === undefined) return false
    for (let i = 0; i < userRoles.length; i++) {
      if (rolesToCheck.some((roleToCheck) => roleToCheck === userRoles[i])) {
        return true
      }
    }
    return false
  }

  const clear = () => {
    isLoggedInRef.value = false
    userResponse.value = undefined
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
    isInAnySpecifiedRole
  }
})
