<template>
  <Panel class="request-password-reset single-panel-center single-panel-center--no-header">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="emailInput"
          class="input__field"
          v-model:model-value="formState.email" />
        <ValidationLabel
          text="Email"
          for="emailInput"
          :message="validation.message('email')" />
      </IftaLabel>
      <div class="buttons">
        <Button
          :loading="isSubmitting"
          :disabled="isSubmitting"
          label="Reset Password"
          @click="handleResetPasswordClick" />
      </div>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { onKeyDown } from '@vueuse/core'
import { IftaLabel, InputText, Button, Panel, useToast } from 'primevue'
import { reactive, ref } from 'vue'

const isSubmitting = ref(false)
const toast = useToast()

const formState = reactive({
  email: ''
})
const validation = createFormValidation(formState, {
  email: combineRules(required(), emailAddress())
})
onKeyDown(KeyName.Enter, () => handleResetPasswordClick())

const handleResetPasswordClick = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.resetPassword.get(formState.email)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  toast.add({
    summary: 'Request received',
    detail:
      'If your email is registered to a user, an email will be sent with a link to reset your password'
  })
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';
.request-password-reset {
  .buttons {
    margin-top: variables.$space-m;
  }
}
</style>
