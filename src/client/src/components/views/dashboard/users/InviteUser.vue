<template>
  <div class="invite-user">
    <FormArea>
      <IftaFormField
        :message="validation.message('email')"
        input-id="emailInput"
        label="Email"
        size="m">
        <InputText
          id="emailInput"
          v-model="formState.email"
          :invalid="!validation.isValid('email')"
          @update:model-value="validation.validateIfDirty('email')" />
      </IftaFormField>
      <GridRowFill />
      <IftaFormField
        :message="validation.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="m">
        <Select
          v-model="formState.communityId"
          :invalid="!validation.isValid('communityId')"
          :options="availableCommunities"
          option-label="name"
          option-value="id"
          @update:model-value="validation.validateIfDirty('communityId')" />
      </IftaFormField>
      <GridRowFill />
      <FormField
        input-id="rolesInput"
        label="Roles"
        size="xs">
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
      <GridRowFill />
      <template #actions>
        <Button
          :disabled="isSubmitting"
          :loading="isSubmitting"
          class="input"
          label="Send Invite"
          @click="handleSubmit" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import { KeyName } from '@/constants/keys'
import { inviteRequestRules } from '@/validation/requestRules'
import { createFormValidation } from '@/validation/types/formValidation'
import { onKeyDown } from '@vueuse/core'
import { Button, Checkbox, InputText, Select, useToast } from 'primevue'
import { computed, onMounted, reactive, ref, watch } from 'vue'
import invitesApi from '@/api/resources/invitesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import FormField from '@/components/form/FormField.vue'
import GridRowFill from '@/components/layout/GridRowFill.vue'
import { useCommunityStore } from '@/stores/communityStore.ts'

const toast = useToast()
const communityStore = useCommunityStore()

const availableCommunities = computed(() =>
  communityStore.communities.filter((community) => community.isOrganizable)
)

onKeyDown(KeyName.Enter, () => handleSubmit())

const formState = reactive({
  email: '',
  communityId: '',
  isPerformer: false,
  isOrganizer: false
})
const validation = createFormValidation(formState, inviteRequestRules)

const props = defineProps<{
  visible?: boolean
}>()

onMounted(async () => {
  if (communityStore.getCommunities().length === 0) {
    await communityStore.loadCommunitiesAsync()
  }
})

const emit = defineEmits<{ create: [] }>()

const isSubmitting = ref(false)
const handleSubmit = async () => {
  const isValid = validation.validate()
  if (!isValid) return

  isSubmitting.value = true
  const response = await invitesApi.post(formState)
  isSubmitting.value = false

  if (tryHandleUnsuccessfulResponse(response, toast, validation)) return

  toast.add({ detail: 'Successfully invited new user: ' + formState.email, severity: 'success' })
  reset()
  emit('create')
}

watch(
  () => props.visible,
  () => {
    reset()
  }
)

const reset = () => {
  formState.email = ''
  formState.communityId = ''
  formState.isPerformer = false
  formState.isOrganizer = false
  validation.reset()
}
</script>
