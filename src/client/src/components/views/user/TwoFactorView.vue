<template>
  <Panel class="two-factor single-panel-center single-panel-center--no-header">
    <Message
      class="single-panel-center__message"
      severity="info">
      A two-factor code has been emailed to you.
    </Message>
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="codeInput"
          v-model:model-value="formState.code"
          :autofocus="true"
          class="input__field" />
        <ValidationLabel
          :disabled="isSubmitting"
          for="usernameInput"
          message=""
          text="Code" />
      </IftaLabel>
      <div v-if="errorMessage">
        <Message
          class="input--medium"
          severity="error">
          {{ errorMessage }}
        </Message>
      </div>
      <div class="buttons">
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          class="input"
          label="Verify"
          @click="handleVerify" />
      </div>
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useAuthStore } from '@/stores/authStore'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Message, Panel } from 'primevue'
import { reactive, ref } from 'vue'
import authApi from '@/api/resources/authApi.ts'

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
