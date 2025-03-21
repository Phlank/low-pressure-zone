<template>
  <Panel class="resend-invite single-panel-center single-panel-center--no-header">
    <Message
      class="single-panel-center__message"
      severity="info">
      Your invitation link is expired. Enter your email to receive a new one.
    </Message>
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="emailInput"
          v-model:model-value="formState.email"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('email')"
          class="input__field"
          @change="validation.validateIfDirty('email')" />
        <ValidationLabel
          :message="validation.message('email')"
          for="emailInput"
          text="Email" />
      </IftaLabel>
      <Button
        :disabled="isSubmitting"
        :loading="isSubmitting"
        class="input"
        label="Resend Invite"
        @click="handleResendInviteClick" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { Button, IftaLabel, InputText, Message, Panel, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import invitesApi from '@/api/resources/invitesApi.ts'

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
