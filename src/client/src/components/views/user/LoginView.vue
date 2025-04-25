<template>
  <SinglePanelViewWrapper class="login-view">
    <FormArea is-single-column>
      <IftaFormField
        input-id="usernameInput"
        label="Username"
        size="m">
        <InputText
          id="usernameInput"
          v-model:model-value="formState.username"
          :autofocus="true"
          :disabled="isSubmitting" />
      </IftaFormField>
      <IftaFormField
        input-id="passwordInput"
        label="Password"
        size="m">
        <Password
          id="passwordInput"
          v-model:model-value="formState.password"
          :disabled="isSubmitting"
          :feedback="false" />
      </IftaFormField>
      <FormField
        v-if="errorMessage"
        input-id=""
        size="m">
        <Message
          severity="error"
          style="width: 100%">
          {{ errorMessage }}
        </Message>
      </FormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Login"
          @click="handleLogin" />
        <Button
          label="Reset Password"
          outlined
          @click="router.push(Routes.RequestPasswordReset)" />
      </template>
    </FormArea>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { loginRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, InputText, Message, Password, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import authApi, { type LoginRequest } from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'

const toast = useToast()
const router = useRouter()

const formState: LoginRequest = reactive({
  username: '',
  password: ''
})
const validationState = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleLogin())

const isSubmitting = ref(false)
const errorMessage = ref('')

withDefaults(
  defineProps<{
    redirect?: string
  }>(),
  {
    redirect: Routes.Schedules
  }
)

const handleLogin = async () => {
  const isValid = validationState.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await authApi.postLogin(formState)
  isSubmitting.value = false
  if (!response.isSuccess()) {
    if (response.status === 403) {
      errorMessage.value = 'Invalid credentials'
      return
    }
    tryHandleUnsuccessfulResponse(response, toast)
  }

  if (response.data().requiresTwoFactor) {
    await router.replace(Routes.TwoFactor)
    return
  }

  await useAuthStore().load()
  isSubmitting.value = false
  await router.push(Routes.Schedules)
}
</script>
