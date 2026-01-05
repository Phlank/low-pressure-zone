import { hasIntersection } from '@/utils/arrayUtils'
import { defineStore } from 'pinia'
import { computed, ref, type Ref, watch} from 'vue'
import authApi, {
  type LoginRequest,
  type TwoFactorRequest,
  type UserInfoResponse
} from '@/api/resources/authApi.ts'
import { err, ok, type Result } from '@/types/result.ts'
import type { FormValidation } from '@/validation/types/formValidation.ts'
import { tryHandleInvalidResponse } from '@/api/tryHandleUnsuccessfulResponse.ts'
import { invalid } from '@/validation/types/validationResult.ts'
import { Routes } from '@/router/routes.ts'
import { LoginOutcome } from '@/constants/loginOutcome.ts'
import router from '@/router'

export const useAuthStore = defineStore('authStore', () => {
  let isInitialized = false
  const isLoggedIn: Ref<boolean> = ref(false)
  const userInfo: Ref<UserInfoResponse> = ref({
    id: '',
    email: '',
    username: '',
    streamerId: null,
    roles: []
  })

  let initializePromise: Promise<void> | null = null
  const initializeAsync = async () => {
    if (isInitialized) return
    if (!initializePromise) {
      initializePromise = reloadAsync()
    }
    await initializePromise
  }
  const reloadAsync = async () => {
    const response = await authApi.getInfo()
    if (response.status === 0) return
    isLoggedIn.value = response.isSuccess()
    if (response.isSuccess()) {
      userInfo.value = response.data()
    }
    isInitialized = true
  }
  if (!isInitialized) {
    initializeAsync().then(() => {})
  }

  const loginAsync = async (
    formState: Ref<LoginRequest>,
    validation: FormValidation<LoginRequest>
  ): Promise<Result<LoginOutcome>> => {
    if (isLoggedIn.value) return err()
    if (!validation.validate()) return err()
    const response = await authApi.postLogin(formState.value)
    if (response.status === 403) {
      validation.setValidity('username', invalid('Invalid credentials'))
      validation.setValidity('password', invalid('Invalid credentials'))
      return err()
    }
    if (tryHandleInvalidResponse(response, validation) || !response.isSuccess()) return err()
    if (response.data().requiresTwoFactor) {
      await router.replace(Routes.TwoFactor)
      return ok(LoginOutcome.RequiresTwoFactor)
    }
    await reloadAsync()
    isLoggedIn.value = true
    return ok(LoginOutcome.LoggedIn)
  }

  const twoFactorLoginAsync = async (
    formState: Ref<TwoFactorRequest>,
    validation: FormValidation<TwoFactorRequest>
  ): Promise<Result<LoginOutcome>> => {
    console.log('2fa')
    console.log(isLoggedIn.value)
    if (isLoggedIn.value) return err()
    console.log('2fa')
    if (!validation.validate()) return err()
    const response = await authApi.postTwoFactor(formState.value)
    if (response.status === 403 || response.status === 401) {
      validation.setValidity('code', invalid('Invalid credentials'))
      return err()
    }
    if (!response.isSuccess()) {
      validation.setValidity('code', invalid('Invalid credentials'))
    }
    await reloadAsync()
    await router.replace(Routes.Schedules)
    return ok(LoginOutcome.LoggedIn)
  }

  const logoutAsync = async (): Promise<Result> => {
    if (!isLoggedIn.value) return err()
    const response = await authApi.getLogout()
    if (!response.isSuccess()) return err()
    reset()
    await router.push(Routes.Home)
    return ok()
  }

  const getIsLoggedIn = computed(() => isLoggedIn.value)

  const getUser = computed(() => userInfo.value)

  const isInRole = (roleToCheck: string): boolean => {
    return userInfo.value.roles.includes(roleToCheck)
  }

  const isInAnyRoles = (...rolesToCheck: string[]): boolean => {
    if (rolesToCheck.length === 0) return true

    return hasIntersection(rolesToCheck, userInfo.value.roles)
  }

  const reset = () => {
    isLoggedIn.value = false
    userInfo.value.id = ''
    userInfo.value.email = ''
    userInfo.value.username = ''
    userInfo.value.roles = []
    userInfo.value.streamerId = null
  }

  return {
    initializeAsync,
    reloadAsync,
    isLoggedIn: getIsLoggedIn,
    user: getUser,
    loginAsync,
    twoFactorLoginAsync,
    logoutAsync,
    isInRole,
    isInAnyRoles,
    reset
  }
})
