<template>
  <div class="community-relationship-form">
    <FormArea v-bind="$attrs">
      <IftaFormField
        :message="validation.message('userId')"
        input-id="userInput"
        label="User"
        size="m">
        <Select
          id="userInput"
          v-model="formState.userId"
          :disabled="props.relationship !== undefined || isSubmitting"
          :invalid="!validation.isValid('userId')"
          :option-label="(user: UserResponse) => user.displayName"
          :option-value="(user: UserResponse) => user.id"
          :options="availableUsers"
          autofocus />
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
          v-if="!props.hideActions"
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Save"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Button, Checkbox, Select } from 'primevue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { computed, onMounted, type Ref, ref } from 'vue'
import {
  type CommunityRelationshipRequest,
  type CommunityRelationshipResponse
} from '@/api/resources/communityRelationshipsApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { alwaysValid, required } from '@/validation/rules/untypedRules.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useAuthStore } from '@/stores/authStore.ts'
import roles from '@/constants/roles.ts'
import { useUserStore } from '@/stores/userStore.ts'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { entitiesExceptIds } from '@/utils/arrayUtils.ts'
import type { Result } from '@/types/result.ts'

const authStore = useAuthStore()
const users = useUserStore()
const communities = useCommunityStore()

const availableUsers = computed(() => {
  if (props.relationship) {
    const user = users.getUser(props.relationship.userId)
    return user ? [user] : []
  }
  const inUse = communities
    .getRelationships(props.communityId)
    .map((relationship) => relationship.userId)
  return entitiesExceptIds(users.users, inUse)
})

const props = withDefaults(
  defineProps<{
    communityId: string
    relationship?: CommunityRelationshipResponse
    hideActions?: boolean
  }>(),
  {
    hideActions: false
  }
)

const formState: Ref<CommunityRelationshipRequest & { userId: string }> = ref({
  userId: '',
  isPerformer: false,
  isOrganizer: false
})
const validation = createFormValidation(formState, {
  userId: required(),
  isPerformer: alwaysValid(),
  isOrganizer: alwaysValid()
})

const isSubmitting = ref(false)
const submit = async () => {
  isSubmitting.value = true
  let result: Result
  if (props.relationship) {
    result = await communities.updateRelationship(
      props.communityId,
      props.relationship.userId,
      formState,
      validation
    )
  } else {
    result = await communities.createRelationship(
      props.communityId,
      formState.value.userId,
      formState,
      validation
    )
  }
  isSubmitting.value = false
  if (!result.isSuccess) return
  reset()
  emit('submitted')
}

const reset = () => {
  formState.value.userId = props.relationship?.userId ?? ''
  formState.value.isPerformer = props.relationship?.isPerformer ?? false
  formState.value.isOrganizer = props.relationship?.isOrganizer ?? false
}

defineExpose({
  formState,
  validation,
  isSubmitting,
  submit,
  reset
})

const emit = defineEmits<{
  submitted: []
}>()

onMounted(() => {
  reset()
})
</script>
