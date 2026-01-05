<template>
  <div class="invite-form">
    <FormArea>
      <IftaFormField
        :message="val.message('email')"
        input-id="emailInput"
        label="Email"
        size="l">
        <InputText
          id="emailInput"
          v-model="state.email"
          :invalid="!val.isValid('email')"
          @update:model-value="val.validateIfDirty('email')" />
      </IftaFormField>
      <IftaFormField
        :message="val.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="l">
        <Select
          v-model="state.communityId"
          :invalid="!val.isValid('communityId')"
          :options="availableCommunities"
          option-label="name"
          option-value="id"
          @update:model-value="val.validateIfDirty('communityId')" />
      </IftaFormField>
      <FormField
        input-id="rolesInput"
        label="Roles"
        size="l">
        <div class="checkbox-area">
          <div class="checkbox-area__item">
            <Checkbox
              id="isPerformerInput"
              v-model="state.isPerformer"
              binary
              name="isPerformer" />
            <label for="isPerformerInput">Performer</label>
          </div>
          <div class="checkbox-area__item">
            <Checkbox
              id="isOrganizerInput"
              v-model="state.isOrganizer"
              binary
              name="isOrganizer" />
            <label for="isOrganizerInput">Organizer</label>
          </div>
        </div>
      </FormField>
      <template #actions>
        <slot name="actions"></slot>
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { Checkbox, InputText, Select } from 'primevue'
import FormArea from '@/components/form/FormArea.vue'
import FormField from '@/components/form/FormField.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'
import { computed, ref } from 'vue'
import { inviteRequestRules } from '@/validation/requestRules.ts'
import { useInviteStore } from '@/stores/inviteStore.ts'
import { useEntityForm } from '@/composables/useEntityForm.ts'

const communityStore = useCommunityStore()
const invites = useInviteStore()
const availableCommunities = computed(() =>
  communityStore.communities.filter((community) => community.isOrganizable)
)

const { state, val, isSubmitting, submit, reset } = useEntityForm({
  validationRules: inviteRequestRules,
  formStateInitializeFn: () => {
    return ref({
      email: '',
      communityId: '',
      isPerformer: false,
      isOrganizer: false
    })
  },
  createPersistentEntityFn: invites.create,
  onSubmitted: () => emit('submitted')
})

const emit = defineEmits<{ submitted: [] }>()

defineExpose({
  submit,
  reset,
  isSubmitting
})
</script>
