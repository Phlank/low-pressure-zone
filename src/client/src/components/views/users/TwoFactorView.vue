<template>
  <Panel class="login">
    <div class="login__form">
      <IftaLabel class="input input--medium">
        <InputText
          :autofocus="true"
          id="codeInput"
          class="input__field"
          v-model:model-value="formState.code" />
        <ValidationLabel
          for="usernameInput"
          message=""
          :disabled="isSubmitting"
          text="Code" />
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
        label="Verify"
        @click="handleVerify" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router, { LOGIN_REDIRECT } from '@/router'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Message, Panel, Password } from 'primevue'
import { reactive, ref } from 'vue'

const formState = reactive({
  code: ''
})

onKeyDown(KeyName.Enter, () => handleVerify())

const isSubmitting = ref(false)
const errorMessage = ref('')

const handleVerify = async () => {
  isSubmitting.value = true
  const response = await api.users.twoFactor.post(formState)
  isSubmitting.value = false
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid code'
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
