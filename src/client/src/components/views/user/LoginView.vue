<template>
  <SinglePanelViewWrapper class="login-view">
    <FormArea is-single-column>
      <IftaFormField
        :message="validation.message('username')"
        input-id="usernameInput"
        label="Username"
        size="m">
        <InputText
          id="usernameInput"
          v-model:model-value="formState.username"
          :autofocus="true"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('username')"
          @change="validation.validateIfDirty('username')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('password')"
        input-id="passwordInput"
        label="Password"
        size="m">
        <Password
          id="passwordInput"
          v-model:model-value="formState.password"
          :disabled="isSubmitting"
          :feedback="false"
          :invalid="!validation.isValid('password')"
          @change="validation.validateIfDirty('password')" />
      </IftaFormField>
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
import { Button, InputText, Password } from 'primevue'
import { type Ref, ref } from 'vue'
import { type LoginRequest } from '@/api/resources/authApi.ts'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'

const router = useRouter()
const auth = useAuthStore()

const formState: Ref<LoginRequest> = ref({
  username: '',
  password: ''
})
const validation = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleLogin())

const isSubmitting = ref(false)

withDefaults(
  defineProps<{
    redirect?: string
  }>(),
  {
    redirect: Routes.Welcome
  }
)

const handleLogin = async () => {
  isSubmitting.value = true
  const result = await auth.loginAsync(formState, validation)
  isSubmitting.value = false
  if (!result.isSuccess) return
  if (result.value) {
    await router.push(Routes.Schedules)
  }
}
</script>
