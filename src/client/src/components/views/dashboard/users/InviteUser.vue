<template>
  <div class="invite-user">
    <IftaLabel class="input input--large">
      <InputText
        class="input__field"
        id="inviteEmailInput"
        v-model="formState.email"
        :invalid="!validation.isValid('email')"
        @change="validation.validateIfDirty('email')" />
      <ValidationLabel
        for="inviteEmailInput"
        :message="validation.message('email')"
        text="Email" />
    </IftaLabel>
    <IftaLabel class="input input--small">
      <Select
        class="input__field"
        id="roleInput"
        :options="
          roleItems.filter((item: RoleItem) => authStore.isInAnySpecifiedRole(...item.availableTo))
        "
        option-label="name"
        option-value="name"
        data-key="name"
        :default-value="Role.Performer"
        v-model:model-value="formState.role" />
      <label for="roleInput">Role</label>
    </IftaLabel>
    <Button
      class="input"
      label="Send Invite"
      :disabled="isSubmitting"
      :loading="isSubmitting"
      @click="handleSubmit" />
  </div>
</template>

<script lang="ts" setup>
import api from '@/api/api'
import { tryHandleUnsuccessfulResponse } from '@/api/apiResponseHandlers'
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { Role } from '@/constants/roles'
import { useAuthStore } from '@/stores/authStore'
import { inviteRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { IftaLabel, InputText, Select, useToast, Button } from 'primevue'
import { reactive, ref, watch } from 'vue'

const toast = useToast()
const authStore = useAuthStore()

onKeyDown(KeyName.Enter, () => handleSubmit())

interface RoleItem {
  name: Role
  availableTo: Role[]
}
const roleItems: RoleItem[] = [
  {
    name: Role.Performer,
    availableTo: [Role.Admin, Role.Organizer]
  },
  {
    name: Role.Organizer,
    availableTo: [Role.Admin, Role.Organizer]
  },
  {
    name: Role.Admin,
    availableTo: [Role.Admin]
  }
]

const formState = reactive({
  email: '',
  role: Role.Performer
})
const validation = createFormValidation(formState, inviteRequestRules)

const props = defineProps<{
  visible?: boolean
}>()

const emit = defineEmits<{ close: [] }>()

const isSubmitting = ref(false)
const handleSubmit = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await api.users.invites.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  toast.add({ detail: 'Successfully invited new user: ' + formState.email, severity: 'success' })
  formState.email = ''
  emit('close')
}

watch(
  () => props.visible,
  (newValue: boolean) => {
    reset()
  }
)

const reset = () => {
  formState.email = ''
  formState.role = Role.Performer
  validation.reset()
}
</script>
