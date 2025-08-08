<template>
  <div class="stream-password-button">
    <Button
      :loading="isPasswordLoading"
      icon="pi pi-key"
      label="Get Password"
      @click="handleGetPassword" />
    <Dialog
      v-model:visible="showPasswordDialog"
      :draggable="false"
      :header="useType === 'live' ? 'New Password' : 'Test Password'"
      modal>
      <template #default>
        <InputText
          :value="password"
          readonly
          style="width: 100%; margin-bottom: 1rem" />
        <div v-if="useType === 'live'">Your old streaming password has been replaced.</div>
      </template>
      <template #footer>
        <div class="stream-information__modal-footer">
          <Button
            :loading="isPasswordLoading"
            icon="pi pi-copy"
            label="Copy Password"
            @click="handleCopyPassword" />
        </div>
      </template>
    </Dialog>
  </div>
</template>

<script lang="ts" setup>
import { Button, Dialog, InputText, useToast } from 'primevue'
import { onMounted, type Ref, ref } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import usersApi from '@/api/resources/usersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import copyToClipboard from '@/utils/copyToClipboard.ts'

const toast = useToast()
const connectionInfoStore = useConnectionInfoStore()
const isPasswordLoading = ref(false)
const showPasswordDialog = ref(false)
const password = ref('')
const isCopySuccessful: Ref<boolean | null> = ref(null)

const props = defineProps<{
  useType: 'live' | 'test'
}>()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})

const handleGetPassword = async () => {
  isCopySuccessful.value = null
  if (props.useType === 'live') {
    await loadLivePasswordAsync()
  } else {
    loadTestPassword()
  }
}

const loadLivePasswordAsync = async () => {
  isPasswordLoading.value = true
  const response = await usersApi.getStreamerPassword()
  isPasswordLoading.value = false
  if (tryHandleUnsuccessfulResponse(response, toast)) return
  password.value = response.data().password
  showPasswordDialog.value = true
}

const loadTestPassword = () => {
  password.value = connectionInfoStore.testInfo()?.password ?? ''
  showPasswordDialog.value = true
}

const handleCopyPassword = async () => {
  const success = await copyToClipboard(password.value)
  if (success) {
    toast.add({
      severity: 'success',
      life: 5000,
      summary: 'Password Copied',
      detail: 'Your new streaming password has been copied to your clipboard.'
    })
  } else {
    toast.add({
      severity: 'error',
      life: 5000,
      summary: 'Failed to Copy Password',
      detail: 'Your new streaming password could not be copied to your clipboard.'
    })
  }
}
</script>
