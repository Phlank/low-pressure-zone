<template>
  <Panel class="login">
    <div
      class="login__form"
      v-if="!isRegistered">
      <Message
        v-if="errorMessage"
        :title="errorCode"
        class="input input--medium">
        {{ errorMessage }}
      </Message>
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
        <Password
          id="passwordInput"
          class="input__field"
          :feedback="false"
          v-model:model-value="formState.password" />
        <ValidationLabel
          for="passwordInput"
          message=""
          text="Password" />
      </IftaLabel>
      <Button
        class="input"
        label="Register"
        @click="handleRegisterClick" />
    </div>
    <div v-else>
      <Message>Successfully registered user.</Message>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import { Panel, IftaLabel, InputText, Button, Password } from 'primevue'
import { reactive, ref } from 'vue'
import { getAuth, createUserWithEmailAndPassword } from 'firebase/auth'
import ValidationLabel from '../form/ValidationLabel.vue'

const formState = reactive({
  email: '',
  password: ''
})

const isRegistered = ref(false)
const errorCode = ref('')
const errorMessage = ref('')

const handleRegisterClick = () => {
  createUserWithEmailAndPassword(getAuth(), formState.email, formState.password)
    .then(() => {
      isRegistered.value = true
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
