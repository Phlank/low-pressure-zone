<template>
  <Panel class="login single-panel-center single-panel-center--no-header">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="usernameInput"
          v-model:model-value="formState.username"
          :autofocus="true"
          :disabled="isSubmitting"
          class="input__field" />
        <ValidationLabel
          for="usernameInput"
          message=""
          text="Username" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          v-model:model-value="formState.password"
          :disabled="isSubmitting"
          :feedback="false"
          class="input__field" />
        <ValidationLabel
          for="passwordInput"
          message=""
          text="Password" />
      </IftaLabel>
      <div v-if="errorMessage">
        <Message
          class="input--medium"
          severity="error">
          {{ errorMessage }}
        </Message>
      </div>
      <div class="buttons">
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Login"
          @click="handleLogin" />
        <Button
          label="Reset"
          outlined
          @click="router.push(Routes.RequestPasswordReset)" />
      </div>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { loginRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Message, Panel, Password } from 'primevue'
import { reactive, ref } from 'vue'
import authApi, { type LoginRequest } from '@/api/resources/authApi.ts'

const formState: LoginRequest = reactive({
  username: '',
  password: ''
})
const validationState = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleLogin())

const isSubmitting = ref(false)
const errorMessage = ref('')

const props = withDefaults(
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
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid credentials'
    isSubmitting.value = false
    return
  }

  if (response.data?.requiresTwoFactor) {
    await router.push('/user/twoFactor')
    return
  }

  await useAuthStore().load()
  isSubmitting.value = false
  await router.push(props.redirect)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';

.login {
  .buttons {
    display: flex;
    max-width: variables.$input-width-m;
    margin-top: variables.$space-m;
    gap: variables.$space-m;
  }
}
</style>
