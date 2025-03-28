<template>
  <div class="community-relationship-form">
    <FormArea>
      <IftaFormField
        v-if="!initialState"
        :message="validation.message('userId')"
        input-id="userInput"
        label="User"
        size="m">
        <Select
          id="userInput"
          v-model="formState.userId"
          :disabled="props.initialState !== undefined"
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
            v-model="formState.isPerformer"
            binary />
          <label for="isPerformerInput">Performer</label>
        </div>
        <div v-if="authStore.isInRole(Role.Admin)">
          <Checkbox
            id="isOrganizerInput"
            v-model="formState.isOrganizer"
            binary />
          <label for="isOrganizerInput">Organizer</label>
        </div>
      </FormField>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Checkbox, Select } from 'primevue'
import { type UserResponse } from '@/api/resources/usersApi.ts'
import { onMounted, reactive } from 'vue'
import type { CommunityRelationshipResponse } from '@/api/resources/communityRelationshipsApi.ts'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { required } from '@/validation/rules/stringRules.ts'
import { alwaysValid } from '@/validation/rules/untypedRules.ts'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useAuthStore } from '@/stores/authStore.ts'
import { Role } from '@/constants/role.ts'

const authStore = useAuthStore()

const props = defineProps<{
  initialState?: CommunityRelationshipResponse
  availableUsers: UserResponse[]
}>()

const formState = reactive({
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
  if (props.initialState) {
    formState.userId = props.initialState.userId
    formState.isPerformer = props.initialState.isPerformer
    formState.isOrganizer = props.initialState.isOrganizer
  } else {
    formState.userId = props.availableUsers[0].id
  }
})

defineExpose({
  formState,
  validation
})
</script>
