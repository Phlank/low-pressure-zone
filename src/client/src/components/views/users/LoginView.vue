<template>
  <Panel class="login">
    <div class="login__form">
      <IftaLabel class="input input--medium">
        <InputText
          :autofocus="true"
          id="usernameInput"
          class="input__field"
          :disabled="isSubmitting"
          v-model:model-value="formState.username" />
        <ValidationLabel
          for="usernameInput"
          message=""
          text="Username" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          class="input__field"
          :feedback="false"
          :disabled="isSubmitting"
          v-model:model-value="formState.password" />
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
      <Button
        class="input"
        label="Login"
        :disabled="isSubmitting"
        @click="handleLogin" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router, { LOGIN_REDIRECT } from '@/router'
import { loginRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Message, Panel, Password } from 'primevue'
import { reactive, ref } from 'vue'

const formState = reactive({
  username: '',
  password: ''
})
const validationState = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleLogin())

const isSubmitting = ref(false)
const errorMessage = ref('')

const handleLogin = async () => {
  const isValid = validationState.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.login.post(formState)
  isSubmitting.value = true
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid credentials entered'
    return
  }

  if (response.data?.requiresTwoFactor) {
    router.push('/user/twofactor')
    return
  }

  router.push(LOGIN_REDIRECT)
}
</script>

<style lang="scss" scoped>
.login {
  margin: auto;
  margin-top: calc(min(20vh));
  width: fit-content;

  &__form {
    display: flex;
    flex-direction: column;
    align-items: center;
  }
}
</style>
