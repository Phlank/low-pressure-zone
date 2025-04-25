<template>
  <SinglePanelViewWrapper>
    <Panel class="password-reset single-panel-center single-panel-center--no-header">
      <div class="single-panel-center__form">
        <IftaLabel class="input input--medium">
          <Password
            id="passwordInput"
            v-model:model-value="formState.password"
            :feedback="false"
            class="input__field"
            @change="validation.validateIfDirty('password')" />
          <ValidationLabel
            :message="validation.message('password')"
            for="passwordInput"
            text="New Password" />
        </IftaLabel>
        <IftaLabel class="input input--medium">
          <Password
            id="confirmPasswordInput"
            v-model:model-value="formState.confirmPassword"
            :feedback="false"
            class="input__field"
            @change="validation.validateIfDirty('confirmPassword')" />
          <ValidationLabel
            :message="validation.message('confirmPassword')"
            for="confirmPasswordInput"
            text="Confirm New Password" />
        </IftaLabel>
        <div class="buttons">
          <Button
            :disabled="isSubmitting"
            :loading="isSubmitting"
            label="Update Password"
            @click="handleUpdatePassword" />
        </div>
      </div>
    </Panel>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { Routes } from '@/router/routes'
import { equals, password, required } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, Panel, Password, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'

const toast = useToast()
const router = useRouter()

const isSubmitting = ref(false)
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
  const response = await authApi.postResetPassword(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast)) return

  toast.add({
    summary: 'Password reset successfully',
    detail: 'Log in with your new password to continue.',
    severity: 'success'
  })

  await router.push(Routes.Login)
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.reset-password {
  .buttons {
    margin-top: variables.$space-m;
  }
}
</style>
