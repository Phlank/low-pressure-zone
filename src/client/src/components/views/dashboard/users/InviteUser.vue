<template>
  <div class="invite-user">
    <IftaLabel class="input input--large">
      <InputText
        id="inviteEmailInput"
        v-model="formState.email"
        :invalid="!validation.isValid('email')"
        class="input__field"
        @change="validation.validateIfDirty('email')" />
      <ValidationLabel
        :message="validation.message('email')"
        for="inviteEmailInput"
        text="Email" />
    </IftaLabel>
    <IftaLabel class="input input--small">
      <Select
        id="roleInput"
        v-model:model-value="formState.role"
        :default-value="Role.Performer"
        :options="roleItems"
        class="input__field"
        data-key="name"
        option-label="name"
        option-value="name" />
      <label for="roleInput">Role</label>
    </IftaLabel>
    <Button
      :disabled="isSubmitting"
      :loading="isSubmitting"
      class="input"
      label="Send Invite"
      @click="handleSubmit" />
  </div>
</template>

<script lang="ts" setup>
import ValidationLabel from '@/components/form/ValidationLabel.vue'
import { KeyName } from '@/constants/keys'
import { Role } from '@/constants/roles'
import { inviteRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, IftaLabel, InputText, Select, useToast } from 'primevue'
import { reactive, ref, watch } from 'vue'
import invitesApi from '@/api/resources/invitesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'

const toast = useToast()

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
  const response = await invitesApi.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  toast.add({ detail: 'Successfully invited new user: ' + formState.email, severity: 'success' })
  formState.email = ''
  emit('close')
}

watch(
  () => props.visible,
  () => {
    reset()
  }
)

const reset = () => {
  formState.email = ''
  formState.role = Role.Performer
  validation.reset()
}
</script>
