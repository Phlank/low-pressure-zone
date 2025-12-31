<template>
  <SinglePanelViewWrapper class="reset-password-view">
    <FormArea is-single-column>
      <IftaFormField
        :message="validation.message('password')"
        input-id="passwordInput"
        label="New Password"
        size="m">
        <Password
          id="passwordInput"
          v-model:model-value="formState.password"
          :feedback="false"
          :invalid="!validation.isValid('password')"
          @change="validation.validateIfDirty('password')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('confirmPassword')"
        input-id="confirmPasswordInput"
        label="Confirm New Password"
        size="m">
        <Password
          id="confirmPasswordInput"
          v-model:model-value="formState.confirmPassword"
          :feedback="false"
          :invalid="!validation.isValid('confirmPassword')"
          @change="validation.validateIfDirty('confirmPassword')" />
      </IftaFormField>
      <FormField
        input-id="passwordHelpTextMessage"
        size="m">
        <Message>
          <div style="display: flex; flex-direction: column; justify-content: start">
            <div>Password must meet the following requirements:</div>
            <ul>
              <li>At least 8 characters in length</li>
              <li>At least one symbol</li>
              <li>At least one uppercase letter</li>
              <li>At least one lowercase letter</li>
              <li>At least one digit</li>
            </ul>
          </div>
        </Message>
      </FormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Update Password"
          @click="handleUpdatePassword" />
      </template>
    </FormArea>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { Routes } from '@/router/routes'
import { equals, password } from '@/validation/rules/stringRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, Message, Password, useToast } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormField from '@/components/form/FormField.vue'
import { required } from '@/validation/rules/untypedRules.ts'

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
