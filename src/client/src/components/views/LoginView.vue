<template>
  <Panel class="login">
    <div class="login__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="emailInput"
          class="input__field"
          v-model:model-value="formState.email" />
        <ValidationLabel
          for="emailInput"
          message=""
          text="Email" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <InputText
          id="passwordInput"
          class="input__field"
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
        @click="handleLoginClick" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import { Panel, IftaLabel, InputText, Button, Message } from 'primevue'
import { reactive, ref } from 'vue'
import ValidationLabel from '../form/ValidationLabel.vue'
import { getAuth, signInWithEmailAndPassword } from 'firebase/auth'
import router from '@/router'

const formState = reactive({
  email: '',
  password: ''
})

const errorCode = ref('')
const errorMessage = ref('')

const handleLoginClick = () => {
  signInWithEmailAndPassword(getAuth(), formState.email, formState.password)
    .then(() => {
      router.push('/dashboard')
    })
    // eslint-disable-next-line @typescript-eslint/no-explicit-any
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
