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
          :invalid="!validation.isValid('email')" />
      </IftaFormField>
      <IftaFormField
        :message="validation.message('communityId')"
        input-id="communityInput"
        label="Community"
        size="m">
        <Select
          :option-label="(data: CommunityResponse) => data.name"
          :option-value="(data: CommunityResponse) => data.id"
          :options="communities"
          model-value="formState.communityId" />
      </IftaFormField>
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
import { Button, InputText, Select, useToast } from 'primevue'
import { onMounted, reactive, type Ref, ref, watch } from 'vue'
import invitesApi from '@/api/resources/invitesApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import communitiesApi, { type CommunityResponse } from '@/api/resources/communitiesApi.ts'

const toast = useToast()

onKeyDown(KeyName.Enter, () => handleSubmit())

const formState = reactive({
  email: '',
  communityId: ''
})
const validation = createFormValidation(formState, inviteRequestRules)

const props = defineProps<{
  visible?: boolean
}>()

const communities: Ref<CommunityResponse[]> = ref([])

onMounted(async () => {
  communities.value =
    (await communitiesApi.get()).data?.filter((community) => community.isOrganizable) ?? []
})

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
  formState.communityId = ''
  validation.reset()
}
</script>
