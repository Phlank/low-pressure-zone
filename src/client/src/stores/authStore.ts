import { hasIntersection } from '@/utils/arrayUtils'
import { defineStore } from 'pinia'
import { ref, type Ref } from 'vue'
import authApi, { type UserInfoResponse } from '@/api/resources/authApi.ts'

export const useAuthStore = defineStore('authStore', () => {
  const isLoggedInRef: Ref<boolean | undefined> = ref(undefined)
  const userInfo: Ref<UserInfoResponse> = ref({
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
    const response = await authApi.getInfo()
    if (response.status === 0) return
    isLoggedInRef.value = response.isSuccess()
    if (response.isSuccess()) {
      userInfo.value = response.data!
    }
  }

  const loadIfNotInitialized = async () => {
    if (isLoggedInRef.value === undefined) await load()
  }

  const isLoggedIn = () => isLoggedInRef.value ?? false

  const getId = () => userInfo.value.id

  const getEmail = () => userInfo.value.email

  const getUsername = () => userInfo.value.username

  const getRoles = () => userInfo.value.roles

  const isInRole = (roleToCheck: string): boolean => {
    return getRoles().includes(roleToCheck)
  }

  const isInAnySpecifiedRole = (...rolesToCheck: string[]): boolean => {
    if (rolesToCheck.length === 0) return true

    return hasIntersection(rolesToCheck, getRoles())
  }

  const clear = () => {
    isLoggedInRef.value = false
    userInfo.value.id = ''
    userInfo.value.email = ''
    userInfo.value.username = ''
    userInfo.value.roles = []
  }

  return {
    load,
    loadIfNotInitialized,
    isLoggedIn,
    getId,
    getEmail,
    getUsername,
    getRoles,
    isInRole,
    /**
     * @returns `true` if the user is in one of the specified roles or no roles were specified.
     */
    isInAnySpecifiedRole,
    clear
  }
})
