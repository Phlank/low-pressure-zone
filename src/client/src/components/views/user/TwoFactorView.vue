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
        :message="errorMessage"
        input-id="codeInput"
        label="Code"
        size="m">
        <InputText
          id="codeInput"
          v-model:model-value="formState.code"
          :autofocus="true"
          :invalid="errorMessage !== ''" />
      </IftaFormField>
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
import { useAuthStore } from '@/stores/authStore'
import { onKeyDown } from '@vueuse/core'
import { Button, InputText, Message } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'

const router = useRouter()

const formState = reactive({
  code: ''
})

onKeyDown(KeyName.Enter, () => handleVerify())

const isSubmitting = ref(false)
const errorMessage = ref('')

const props = withDefaults(
  defineProps<{
    redirect?: string
  }>(),
  {
    redirect: Routes.Schedules
  }
)

const handleVerify = async () => {
  isSubmitting.value = true
  const response = await authApi.postTwoFactor(formState.code)
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid code'
    isSubmitting.value = false
    return
  }

  await useAuthStore().load()
  await router.push(props.redirect)
}
</script>
