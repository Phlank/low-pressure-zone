<template>
  <div class="community-relationship-form">
    <FormArea :align-actions="alignActions">
      <IftaFormField
        :message="validation.message('userId')"
        input-id="userInput"
        label="User"
        size="m">
        <Select
          id="userInput"
          v-model="formState.userId"
          :disabled="props.initialState !== undefined || isSubmitting"
          :invalid="!validation.isValid('userId')"
          :option-label="(user: UserResponse) => user.displayName"
          :option-value="(user: UserResponse) => user.id"
          :options="availableUsers" />
      </IftaFormField>
      <FormField
        input-id="rolesInput"
        label="Roles"
        size="m">
        <div>
          <Checkbox
            id="isPerformerInput"
            v-model:model-value="formState.isPerformer"
            :disabled="isSubmitting"
            binary />
          <label for="isPerformerInput">Performer</label>
        </div>
        <div v-if="authStore.isInRole(roles.admin)">
          <Checkbox
            id="isOrganizerInput"
            v-model:model-value="formState.isOrganizer"
            :disabled="isSubmitting"
            binary />
          <label for="isOrganizerInput">Organizer</label>
        </div>
      </FormField>
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Button, Checkbox, Select, useToast } from 'primevue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { onMounted, ref } from 'vue'
import communityRelationshipsApi, {
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { required } from '@/validation/rules/stringRules.ts'
import { alwaysValid } from '@/validation/rules/untypedRules.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useUserStore } from '@/stores/userStore.ts'
import { showSuccessToast } from '@/utils/toastUtils.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'

const authStore = useAuthStore()
const userStore = useUserStore()
const communityStore = useCommunityStore()
const toast = useToast()

const props = withDefaults(
  defineProps<{
    communityId: string
    relationshipId?: string
    initialState?: CommunityRelationshipResponse
    availableUsers: UserResponse[]
    alignActions?: 'left' | 'right'
  }>(),
  {
    alignActions: 'left'
  }
)

const formState = ref({
  userId: '',
  isPerformer: false,
  isOrganizer: false
})

const validation = createFormValidation(formState, {
  userId: required(),
  isPerformer: alwaysValid(),
  isOrganizer: alwaysValid()
})

onMounted(async () => {
  reset()
})

const reset = () => {
  formState.value.userId = props.initialState?.userId ?? props.availableUsers[0].id
  formState.value.isPerformer = props.initialState?.isPerformer ?? false
  formState.value.isOrganizer = props.initialState?.isOrganizer ?? false
}

const isSubmitting = ref(false)
const submit = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  const result = await communityRelationshipsApi.put(
    props.communityId,
    formState.value.userId,
    formState.value
  )
  isSubmitting.value = false
  if (tryHandleUnsuccessfulResponse(result, toast, validation)) return
  showSuccessToast(
    toast,
    'Updated',
    'Relationship',
    userStore.getUser(formState.value.userId)?.displayName ?? ''
  )
  await communityStore.loadRelationshipsAsync(props.communityId)
  emits('afterSubmit')
}

const emits = defineEmits<{
  afterSubmit: []
}>()
</script>
