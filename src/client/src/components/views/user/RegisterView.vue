<template>
  <Panel
    class="register single-panel-center"
    header="Register User">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="usernameInput"
          v-model:model-value="formState.username"
          :invalid="!validation.isValid('username')"
          class="input__field"
          @change="validation.validateIfDirty('username')" />
        <ValidationLabel
          :message="validation.message('username')"
          for="usernameInput"
          text="Username" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          v-model:model-value="formState.password"
          :feedback="false"
          :invalid="!validation.isValid('password')"
          class="input__field"
          @change="validation.validateIfDirty('password')" />
        <ValidationLabel
          :message="validation.message('password')"
          for="passwordInput"
          text="Password" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="confirmPasswordInput"
          v-model:model-value="formState.confirmPassword"
          :feedback="false"
          :invalid="!validation.isValid('confirmPassword')"
          class="input__field"
          @change="validation.validateIfDirty('confirmPassword')" />
        <ValidationLabel
          :message="validation.message('confirmPassword')"
          for="confirmPasswordInput"
          text="Confirm Password" />
      </IftaLabel>
      <Button
        :disabled="isSubmitting"
        :loading="isSubmitting"
        class="input"
        label="Register"
        @click="handleRegister" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { TokenProvider, TokenPurpose } from '@/constants/tokens'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { registerRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Panel, Password, useToast } from 'primevue'
import { onMounted, reactive, ref } from 'vue'
import authApi, { type RegisterRequest } from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

onKeyDown(KeyName.Enter, () => handleRegister())

const props = defineProps<{
  context: string
}>()

const formState: RegisterRequest = reactive({
  context: '',
  username: '',
  password: '',
  confirmPassword: ''
})
const validation = createFormValidation(formState, registerRequestRules(formState))

const authStore = useAuthStore()
onMounted(async () => {
  if (!props.context) {
    await router.replace(Routes.Home)
  }
  await authApi.getLogout()
  authStore.clear()

  const response = await authApi.getVerifyToken({
    context: props.context,
    purpose: TokenPurpose.Invite,
    provider: TokenProvider.Default
  })
  if (!response.isSuccess()) {
    await router.replace(Routes.ResendInvite)
    return
  }
  formState.context = props.context
})

const isSubmitting = ref(false)
const toast = useToast()
const handleRegister = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await authApi.postRegister(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  toast.add({ detail: 'Successfully registered.', severity: 'success' })
  await router.push(Routes.Login)
}
</script>
