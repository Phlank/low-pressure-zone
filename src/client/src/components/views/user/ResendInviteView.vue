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
          class="input__field"
          :disabled="isSubmitting"
          :invalid="!validation.isValid('email')"
          @change="validation.validateIfDirty('email')"
          v-model:model-value="formState.email" />
        <ValidationLabel
          for="usernameInput"
          :message="validation.message('email')"
          text="Email" />
      </IftaLabel>
      <Button
        class="input"
        label="Resend Invite"
        @click="handleResendInviteClick"
        :disabled="isSubmitting"
        :loading="isSubmitting" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { emailAddress, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { combineRules } from '@/validation/types/validationRule'
import { IftaLabel, Panel, InputText, Button, Message, useToast } from 'primevue'
import { reactive, ref } from 'vue'

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
  isSubmitting.value = true
  const response = await api.users.invites.resend(formState.email)
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

const props = defineProps<{
  showExpiredTokenMessage?: boolean
}>()
</script>
