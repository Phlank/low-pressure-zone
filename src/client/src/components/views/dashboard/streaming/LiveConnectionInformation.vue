<template>
  <div class="live-connection-information">
    <h4>Connection Information</h4>
    <div>
      <ListItem>
        <template #left>Host/Server</template>
        <template #right
          ><kbd>{{ connectionInfoStore.liveInfo()?.host }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Port</template>
        <template #right
          ><kbd>{{ connectionInfoStore.liveInfo()?.port }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Mount</template>
        <template #right
          ><kbd>{{ connectionInfoStore.liveInfo()?.mount }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Username</template>
        <template #right
          ><kbd>{{ connectionInfoStore.liveInfo()?.username }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Password</template>
        <template #right>
          <Button
            :loading="isPasswordLoading"
            icon="pi pi-key"
            label="Get Password"
            @click="handleGetPassword" />
        </template>
      </ListItem>
    </div>
    <Dialog
      v-model:visible="showPasswordDialog"
      :draggable="false"
      header="New Password"
      modal
      @hide="handleCloseDialog">
      <template #default>
        <InputText
          :value="password"
          readonly
          style="width: 100%; margin-bottom: 1rem" />
        <div>Your old streaming password has been replaced.</div>
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
import { Button, Dialog, Divider, InputText, useToast } from 'primevue'
import ListItem from '@/components/data/ListItem.vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { ref } from 'vue'
import usersApi from '@/api/resources/usersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import copyToClipboard from '@/utils/copyToClipboard.ts'

const toast = useToast()
const connectionInfoStore = useConnectionInfoStore()
const isPasswordLoading = ref(false)
const showPasswordDialog = ref(false)
const password = ref('')
const isCopySuccessful = ref(null)

const handleGetPassword = async () => {
  isCopySuccessful.value = null
  isPasswordLoading.value = true
  const response = await usersApi.getStreamerPassword()
  isPasswordLoading.value = false
  if (tryHandleUnsuccessfulResponse(response, toast)) return
  password.value = response.data().password
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

const handleCloseDialog = () => {
  showPasswordDialog.value = false
  isCopySuccessful.value = null
  password.value = ''
}
</script>
