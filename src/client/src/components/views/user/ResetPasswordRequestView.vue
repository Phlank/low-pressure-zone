<template>
  <SinglePanelViewWrapper class="reset-password-request-view">
    <FormArea is-single-column>
      <IftaFormField
        :message="validation.message('email')"
        input-id="emailInput"
        label="Email">
        <InputText
          id="emailInput"
          v-model:model-value="formState.email"
          :invalid="!validation.isValid('email')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Reset Password"
          @click="handleResetPasswordClick" />
      </template>
    </FormArea>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { onKeyDown } from '@vueuse/core'
import { Button, InputText, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'

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
