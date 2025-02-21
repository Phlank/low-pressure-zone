<template>
  <Panel class="setup-user single-panel-center">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText :autofocus="true" id="usernameInput" class="input__field" :disabled="isSubmitting"
          v-model:model-value="formState.username" />
        <ValidationLabel for="usernameInput" message="" text="Username" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password id="passwordInput" class="input__field" :feedback="false" :disabled="isSubmitting"
          v-model:model-value="formState.password" />
        <ValidationLabel for="passwordInput" message="" text="Password" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password id="confirmPasswordInput" class="input__field" :feedback="false" :disabled="isSubmitting"
          v-model:model-value="formState.password" />
        <ValidationLabel for="confirmPasswordInput" message="" text="Password" />
      </IftaLabel>
      <Button class="input" label="Setup User" :disabled="isSubmitting" @click="handleSetupUser" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import router, { defaultLoginRedirect } from '@/router'
import { useUserStore } from '@/stores/userStore'
import { loginRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Panel, Password } from 'primevue'
import { reactive, ref } from 'vue'

const formState = reactive({
  username: '',
  password: '',
  confirmPassword: ''
})
const validationState = createFormValidation(formState, loginRequestRules)

onKeyDown(KeyName.Enter, () => handleSetupUser())

const isSubmitting = ref(false)
const errorMessage = ref('')

const handleSetupUser = async () => {
  const isValid = validationState.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.login(formState)
  if (!response.isSuccess()) {
    errorMessage.value = 'Invalid credentials'
    isSubmitting.value = false
    return
  }

  if (response.data?.requiresTwoFactor) {
    router.push('/user/twofactor')
    return
  }

  await useUserStore().load()
  isSubmitting.value = false
  router.push(defaultLoginRedirect)
}
</script>
