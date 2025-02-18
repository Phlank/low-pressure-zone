<template>
  <Panel class="two-factor single-panel-center">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          :autofocus="true"
          id="codeInput"
          class="input__field"
          v-model:model-value="formState.code" />
        <ValidationLabel
          for="usernameInput"
          message=""
          :disabled="isSubmitting"
          text="Code" />
      </IftaLabel>
      <div v-if="errorMessage">
        <Message
          class="input--medium"
          severity="error">
          {{ errorMessage }}
        </Message>
      </div>
      <Button
        class="input"
        label="Verify"
        @click="handleVerify" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router, { defaultLoginRedirect } from '@/router'
import { useUserStore } from '@/stores/userStore'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Message, Panel, Checkbox } from 'primevue'
import { reactive, ref } from 'vue'

const formState = reactive({
  code: '',
  rememberMe: false
})

onKeyDown(KeyName.Enter, () => handleVerify())

const isSubmitting = ref(false)
const errorMessage = ref('')

const props = defineProps<{
  redirect: string
}>()

const handleVerify = async () => {
  isSubmitting.value = true
  const response = await api.users.twoFactor.post(formState)
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid code'
    isSubmitting.value = false
    return
  }

  await useUserStore().load()
  isSubmitting.value = false
  router.push(props.redirect ?? defaultLoginRedirect)
}
</script>
