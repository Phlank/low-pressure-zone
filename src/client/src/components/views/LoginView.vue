<template>
  <Panel class="login">
    <div class="login__form">
      <IftaLabel class="input input--medium">
        <InputText
          :autofocus="true"
          id="emailInput"
          class="input__field"
          v-model:model-value="formState.email" />
        <ValidationLabel
          for="emailInput"
          message=""
          text="Email" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          class="input__field"
          :feedback="false"
          v-model:model-value="formState.password" />
        <ValidationLabel
          for="emailInput"
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
        @click="handleLogin" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import { Panel, IftaLabel, InputText, Button, Message, Password, KeyFilter } from 'primevue'
import { reactive, ref } from 'vue'
import ValidationLabel from '../form/ValidationLabel.vue'
import { getAuth, signInWithEmailAndPassword } from 'firebase/auth'
import router from '@/router'
import { onKeyDown } from '@vueuse/core'
import { KeyName } from '@/constants/keys'
import { createFormValidation } from '@/validation/types/formValidation'
import { loginRequestRules } from '@/validation/requestRules'

const formState = reactive({
  email: '',
  password: ''
})
const validationState = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleLogin())

const errorCode = ref('')
const errorMessage = ref('')

const handleLogin = () => {
  const isValid = validationState.validate()
  if (!isValid) return

  signInWithEmailAndPassword(getAuth(), formState.email, formState.password)
    .then(() => {
      router.push('/dashboard')
    })
    .catch((reason: any) => {
      errorCode.value = reason.code
      errorMessage.value = reason.message
    })
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
