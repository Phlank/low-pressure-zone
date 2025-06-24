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
      <FormField input-id="rememberBrowserInput">
        <div style="display: flex; flex-direction: column; align-items: center; width: 100%">
          <div class="checkbox-area__item">
            <Checkbox
              id="rememberBrowserInput"
              v-model="formState.rememberClient"
              binary />
            <span>Remember Browser</span>
          </div>
        </div>
      </FormField>
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
import { Button, Checkbox, InputText, Message } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useRouter } from 'vue-router'
import SinglePanelViewWrapper from '@/components/layout/SinglePanelViewWrapper.vue'
import { useScheduleStore } from '@/stores/scheduleStore.ts'

const router = useRouter()

const formState = reactive({
  code: '',
  rememberClient: false
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
  const response = await authApi.postTwoFactor(formState)
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid code'
    isSubmitting.value = false
    return
  }

  await useAuthStore().load()
  await useScheduleStore().loadDefaultSchedulesAsync()
  await router.push(props.redirect)
}
</script>
