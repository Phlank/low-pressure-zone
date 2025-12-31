<template>
  <div class="stream-credentials">
    <FormArea>
      <IftaFormField
        :message="validation.message('displayName')"
        input-id="playButtonTextInput"
        label="Play Button Text"
        size="m">
        <InputText
          id="playButtonTextInput"
          v-model="formState.displayName"
          :invalid="!validation.isValid('displayName')"
          autofocus />
      </IftaFormField>
      <template #actions>
        <Button
          :loading="isSubmitting"
          label="Save"
          @click="handleSave" />
      </template>
    </FormArea>
  </div>
</template>

<script lang="ts" setup>
import FormArea from '@/components/form/FormArea.vue'
import IftaFormField from '@/components/form/IftaFormField.vue'
import { Button, InputText, useToast } from 'primevue'
import { onMounted, type Ref, ref } from 'vue'
import { streamerRequestRules } from '@/validation/requestRules.ts'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import usersApi, { type StreamerRequest } from '@/api/resources/usersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { showEditSuccessToast } from '@/utils/toastUtils.ts'

const toast = useToast()

const connectionInfoStore = useConnectionInfoStore()
const formState: Ref<StreamerRequest> = ref({ displayName: '' })
const validation = createFormValidation(formState, streamerRequestRules)
const isSubmitting = ref(false)

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
  formState.value.displayName = connectionInfoStore.liveInfo()?.displayName ?? ''
})

const handleSave = async () => {
  if (!validation.validate()) return
  isSubmitting.value = true
  const updateResponse = await usersApi.putStreamer(formState.value)
  if (tryHandleUnsuccessfulResponse(updateResponse, toast, validation)) {
    isSubmitting.value = false
    return
  }
  await connectionInfoStore.load()
  isSubmitting.value = false
  showEditSuccessToast(toast, 'stream settings')
}
</script>
