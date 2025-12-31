<template>
  <div class="invite-form">
    <FormArea>
      <IftaFormField
        :message="validation.message('email')"
        input-id="emailInput"
        label="Email"
        size="l">
        <InputText
          id="emailInput"
          v-model="formState.email"
          :invalid="!validation.isValid('email')"
          @update:model-value="validation.validateIfDirty('email')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="l">
        <Select
          v-model="formState.communityId"
          :invalid="!validation.isValid('communityId')"
          :options="availableCommunities"
          option-label="name"
          option-value="id"
          @update:model-value="validation.validateIfDirty('communityId')" />
      </IftaFormField>
      <FormField
        input-id="rolesInput"
        label="Roles"
        size="l">
        <div class="checkbox-area">
          <div class="checkbox-area__item">
            <Checkbox
              id="isPerformerInput"
              v-model="formState.isPerformer"
              binary
              name="isPerformer" />
            <label for="isPerformerInput">Performer</label>
          </div>
          <div class="checkbox-area__item">
            <Checkbox
              id="isOrganizerInput"
              v-model="formState.isOrganizer"
              binary
              name="isOrganizer" />
            <label for="isOrganizerInput">Organizer</label>
          </div>
        </div>
      </FormField>
      <template #actions>
        <Button
          v-if="!hideSubmit"
          :disabled="isSubmitting"
          :loading="isSubmitting"
          label="Send Invite"
          @click="submit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Button, Checkbox, InputText, Select } from 'primevue'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { computed, ref, type Ref } from 'vue'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import { inviteRequestRules } from '@/validation/requestRules.ts'
import { type InviteRequest } from '@/api/resources/invitesApi.ts'
import { useInviteStore } from '@/stores/inviteStore.ts'

const communityStore = useCommunityStore()
const invites = useInviteStore()
const availableCommunities = computed(() =>
  communityStore.communities.filter((community) => community.isOrganizable)
)

withDefaults(defineProps<{
  hideSubmit: boolean
}>(), {
  hideSubmit: false
})

const formState: Ref<InviteRequest> = ref({
  email: '',
  communityId: '',
  isPerformer: false,
  isOrganizer: false
})
const validation = createFormValidation(formState, inviteRequestRules)

const isSubmitting = ref(false)
const submit = async () => {
  isSubmitting.value = true
  if (!validation.validate()) return
  const result = await invites.create(formState, validation)
  isSubmitting.value = false
  if (!result.isSuccess) return
  reset()
  emit('submitted')
}

const reset = () => {
  formState.value.email = ''
  formState.value.communityId = ''
  formState.value.isPerformer = false
  formState.value.isOrganizer = false
  validation.reset()
}

const emit = defineEmits<{ submitted: [] }>()

defineExpose({
  submit,
  reset,
  isSubmitting,
})
</script>
