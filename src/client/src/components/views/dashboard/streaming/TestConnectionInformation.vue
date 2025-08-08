<template>
  <div class="test-connection-information">
    <h4>Connection Information</h4>
    <div>
      <ListItem>
        <template #left>Host/Server</template>
        <template #right
          ><kbd>{{ connectionInfoStore.testInfo()?.host }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Port</template>
        <template #right
          ><kbd>{{ connectionInfoStore.testInfo()?.port }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Mount</template>
        <template #right
          ><kbd>{{ connectionInfoStore.testInfo()?.mount }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Username</template>
        <template #right
          ><kbd>{{ connectionInfoStore.testInfo()?.username }}</kbd></template
        >
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Password</template>
        <template #right>
          <Button
            icon="pi pi-key"
            label="Get Password"
            @click="() => (showPasswordDialog = true)" />
        </template>
      </ListItem>
    </div>
    <Dialog
      v-model:visible="showPasswordDialog"
      :draggable="false"
      header="Test Password"
      modal>
      <template #default>
        <InputText
          :value="connectionInfoStore.testInfo()?.password"
          readonly
          style="width: 100%; margin-bottom: 1rem" />
      </template>
      <template #footer>
        <div class="stream-information__modal-footer">
          <Button
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
import copyToClipboard from '@/utils/copyToClipboard.ts'
import { ref } from 'vue'

const toast = useToast()
const connectionInfoStore = useConnectionInfoStore()
const showPasswordDialog = ref(false)

const handleCopyPassword = async () => {
  const success = await copyToClipboard(connectionInfoStore.testInfo()?.password ?? '')
  if (success) {
    toast.add({
      severity: 'success',
      life: 7000,
      summary: 'Password Copied',
      detail: 'Your new streaming password has been copied to your clipboard.'
    })
  } else {
    toast.add({
      severity: 'error',
      life: 7000,
      summary: 'Failed to Copy Password',
      detail: 'Your new streaming password could not be copied to your clipboard.'
    })
  }
}
</script>
