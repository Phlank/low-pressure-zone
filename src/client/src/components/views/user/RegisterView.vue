<template>
  <Panel
    class="register single-panel-center"
    header="Register User">
    <div class="single-panel-center__form">
      <IftaLabel class="input input--medium">
        <InputText
          id="usernameInput"
          class="input__field"
          :invalid="!validation.isValid('username')"
          @change="validation.validateIfDirty('username')"
          v-model:model-value="formState.username" />
        <ValidationLabel
          for="usernameInput"
          :message="validation.message('username')"
          text="Username" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="passwordInput"
          class="input__field"
          :feedback="false"
          :invalid="!validation.isValid('password')"
          @change="validation.validateIfDirty('password')"
          v-model:model-value="formState.password" />
        <ValidationLabel
          for="passwordInput"
          :message="validation.message('password')"
          text="Password" />
      </IftaLabel>
      <IftaLabel class="input input--medium">
        <Password
          id="confirmPasswordInput"
          class="input__field"
          :feedback="false"
          :invalid="!validation.isValid('confirmPassword')"
          @change="validation.validateIfDirty('confirmPassword')"
          v-model:model-value="formState.confirmPassword" />
        <ValidationLabel
          for="confirmPasswordInput"
          :message="validation.message('confirmPassword')"
          text="Confirm Password" />
      </IftaLabel>
      <Button
        class="input"
        label="Register"
        :disabled="isSubmitting"
        @click="handleRegister" />
    </div>
  </Panel>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import router from '@/router'
import { Routes } from '@/router/routes'
import { useUserStore } from '@/stores/userStore'
import { registerRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { Button, IftaLabel, InputText, Panel, Password, useToast } from 'primevue'
import { onMounted, reactive, ref } from 'vue'

const props = defineProps<{
  context: string
}>()

const formState = reactive({
  context: props.context,
  username: '',
  password: '',
  confirmPassword: ''
})
const validation = createFormValidation(formState, registerRequestRules(formState))

const userStore = useUserStore()
onMounted(async () => {
  if (!props.context) {
    router.replace(Routes.Home)
  }
  await api.users.logout()
  userStore.clear()
})

const isSubmitting = ref(false)
const toast = useToast()
const handleRegister = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.register(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  toast.add({ detail: 'Successfully registered.', severity: 'success' })
  router.push(Routes.Login)
}
</script>
