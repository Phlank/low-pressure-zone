<template>
  <Panel
    class="register single-panel-center"
    header="Register User">
    <FormArea>
      <IftaFormField
        :message="validation.message('username')"
        input-id="usernameInput"
        label="Username"
        size="m">
        <InputText
          id="usernameInput"
          v-model="formState.username"
          :invalid="!validation.isValid('username')"
          @update:model-value="validation.validateIfDirty('username')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('displayName')"
        input-id="displayNameInput"
        label="Display Name"
        size="m">
        <InputText
          id="displayNameInput"
          v-model="formState.displayName"
          :invalid="!validation.isValid('displayName')"
          @update:model-value="validation.validateIfDirty('displayName')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('password')"
        input-id="passwordInput"
        label="Password"
        size="m">
        <Password
          id="passwordInput"
          v-model="formState.password"
          :feedback="false"
          :invalid="!validation.isValid('password')"
          type="password"
          @update:model-value="validation.validateIfDirty('password')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('confirmPassword')"
        input-id="confirmPasswordInput"
        label="Confirm Password"
        size="m">
        <Password
          id="confirmPasswordInput"
          v-model="formState.confirmPassword"
          :feedback="false"
          :invalid="!validation.isValid('confirmPassword')"
          type="password"
          @update:model-value="validation.validateIfDirty('confirmPassword')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          class="input"
          label="Register"
          @click="handleRegister" />
      </template>
    </FormArea>
  </Panel>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { TokenProvider, TokenPurpose } from '@/constants/tokens'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { registerRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, InputText, Panel, Password, useToast } from 'primevue'
import { onMounted, reactive, ref } from 'vue'
import authApi, { type RegisterRequest } from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useRouter } from 'vue-router'

const router = useRouter()

onKeyDown(KeyName.Enter, () => handleRegister())

const props = defineProps<{
  context: string
}>()

const formState: RegisterRequest = reactive({
  context: '',
  username: '',
  displayName: '',
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
