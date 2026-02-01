<template>
  <SinglePanelViewWrapper class="two-factor-view">
    <FormArea is-single-column>
      <FormField
        input-id="none"
        size="m">
        <Message
          class="single-panel-center__message"
          severity="info">
          A two-factor code has been emailed to you.
        </Message>
      </FormField>
      <IftaFormField
        :message="validation.message('code')"
        input-id="codeInput"
        label="Code"
        size="m">
        <InputText
          id="codeInput"
          v-model:model-value="formState.code"
          :autofocus="true"
          :invalid="!validation.isValid('code')" />
      </IftaFormField>
      <InlineFormField
        input-id="rememberBrowserInput"
        label="Remember Browser">
        <ToggleSwitch
          v-model="formState.rememberClient"
          input-id="rememberBrowserInput" />
      </InlineFormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Verify"
          @click="handleVerify" />
      </template>
    </FormArea>
  </SinglePanelViewWrapper>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { Routes } from '@/router/routes'
import { onKeyDown } from '@vueuse/core'
import { Button, InputText, Message, ToggleSwitch } from 'primevue'
import { type Ref, ref } from 'vue'
import { type TwoFactorRequest } from '@/api/resources/authApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { alwaysValid, required } from '@/validation/rules/untypedRules.ts'
import { useAuthStore } from '@/stores/authStore.ts'
import InlineFormField from '@/components/form/InlineFormField.vue'

onKeyDown(KeyName.Enter, () => handleVerify())

const formState: Ref<TwoFactorRequest> = ref({
  code: '',
  rememberClient: false
})
const validation = createFormValidation(formState, {
  code: required(),
  rememberClient: alwaysValid()
})

withDefaults(
  defineProps<{
    redirect?: string
  }>(),
  {
    redirect: Routes.Welcome
  }
)

const isSubmitting = ref(false)
const auth = useAuthStore()
const handleVerify = async () => {
  isSubmitting.value = true
  await auth.twoFactorLoginAsync(formState, validation)
  isSubmitting.value = false
}
</script>
