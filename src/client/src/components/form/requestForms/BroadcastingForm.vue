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
          :invalid="!validation.isValid('displayName')" />
      </IftaFormField>
      <template #actions>
        <Button
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
import { type Reactive, reactive } from 'vue'
import type { ConnectionInformationResponse } from '@/api/resources/streamApi.ts'
import { streamerRequestRules } from '@/validation/requestRules.ts'
import { createFormValidation } from '@/validation/types/formValidation.ts'
import usersApi, { type StreamerRequest } from '@/api/resources/usersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { showEditSuccessToast } from '@/utils/toastUtils.ts'

const toast = useToast()
const props = defineProps<{
  info: ConnectionInformationResponse
}>()
const connectionInfoStore = useConnectionInfoStore()
const formState: Reactive<StreamerRequest> = reactive({ displayName: props.info.displayName })
const validation = createFormValidation(formState, streamerRequestRules)

const handleSave = async () => {
  if (!validation.validate()) return
  const updateResponse = await usersApi.putStreamer(formState)
  if (tryHandleUnsuccessfulResponse(updateResponse, toast, validation)) return
  await connectionInfoStore.load()
  showEditSuccessToast(toast, 'stream settings')
}
</script>
