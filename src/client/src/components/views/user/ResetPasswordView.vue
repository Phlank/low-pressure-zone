<template>
  <Panel class="password-reset single-panel-center single-panel-center--no-header">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          class="input__field"
          v-model:model-value="formState.password"
          :feedback="false"
          @change="validation.validateIfDirty('password')" />
        <ValidationLabel
          for="passwordInput"
          text="New Password"
          :message="validation.message('password')" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="confirmPasswordInput"
          class="input__field"
          v-model:model-value="formState.confirmPassword"
          :feedback="false"
          @change="validation.validateIfDirty('confirmPassword')" />
        <ValidationLabel
          for="confirmPasswordInput"
          text="Confirm New Password"
          :message="validation.message('confirmPassword')" />
      </IftaLabel>
      <div class="buttons">
        <Button
          label="Update Password"
          :loading="isSubmitting"
          :disabled="isSubmitting"
          @click="handleUpdatePassword" />
      </div>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router from '@/router'
import { Routes } from '@/router/routes'
import { equals, password, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Panel, IftaLabel, Password, Button, useToast } from 'primevue'
import { reactive, ref } from 'vue'

const isSubmitting = ref(false)
const toast = useToast()
onKeyDown(KeyName.Enter, () => handleUpdatePassword())
const props = defineProps<{
  context: string
}>()

const formState = reactive({
  context: props.context,
  password: '',
  confirmPassword: ''
})

const validation = createFormValidation(formState, {
  context: required(),
  password: password,
  confirmPassword: equals(() => formState.password)
})

const handleUpdatePassword = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.resetPassword.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  toast.add({
    summary: 'Password reset successfully',
    detail: 'Log in with your new password to continue.',
    severity: 'success'
  })

  router.push(Routes.Login)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables.scss';
.reset-password {
  .buttons {
    margin-top: variables.$space-m;
  }
}
</style>
