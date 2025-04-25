<template>
  <SinglePanelViewWrapper>
    <FormArea is-single-column>
      <FormField
        input-id=""
        size="m">
        <Message
          severity="info"
          style="width: 100%">
          Your invitation link is expired. Enter your email to receive a new one.
        </Message>
      </FormField>
      <IftaFormField
        :message="validation.message('email')"
        input-id="emailInput"
        label="Email"
        size="m">
        <InputText
          id="emailInput"
          v-model:model-value="formState.email"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('email')"
          class="input__field"
          @change="validation.validateIfDirty('email')" />
      </IftaFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          class="input"
          label="Resend Invite"
          @click="handleResendInviteClick" />
      </template>
    </FormArea>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { Button, InputText, Message, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import invitesApi from '@/api/resources/invitesApi.ts'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'

const toast = useToast()
const isSubmitting = ref(false)

const formState = reactive({
  email: ''
})
const validation = createFormValidation(formState, {
  email: combineRules(required(), emailAddress())
})

const handleResendInviteClick = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await invitesApi.getResend(formState.email)
  isSubmitting.value = false

  if (!response.isSuccess()) {
    tryHandleUnsuccessfulResponse(response, toast)
  }
  toast.add({
    summary: 'Request received',
    detail:
      'If your email address has been invited, a new invitation has been sent to your email address.',
    severity: 'success'
  })
}

defineProps<{
  showExpiredTokenMessage?: boolean
}>()
</script>
