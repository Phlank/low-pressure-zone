<template>
  <SinglePanelViewWrapper>
    <Panel class="request-password-reset single-panel-center single-panel-center--no-header">
      <div class="single-panel-center__form">
        <IftaLabel class="input input--medium">
          <InputText
            id="emailInput"
            v-model:model-value="formState.email"
            class="input__field" />
          <ValidationLabel
            :message="validation.message('email')"
            for="emailInput"
            text="Email" />
        </IftaLabel>
        <div class="single-panel-center__form__buttons">
          <Button
            :disabled="isSubmitting"
            :loading="isSubmitting"
            label="Reset Password"
            @click="handleResetPasswordClick" />
        </div>
      </div>
    </Panel>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Panel, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'

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
  const response = await authApi.getResetPassword(formState.email)
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
@use '@/assets/styles/variables';

.request-password-reset {
  .buttons {
    margin-top: variables.$space-m;
  }
}
</style>
