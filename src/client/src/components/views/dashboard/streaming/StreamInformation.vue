<template>
  <div class="stream-information">
    <div class="stream-information__live">
      <h2>Live Stream</h2>
      <h4>Broadcasting</h4>
      <BroadcastingForm
        v-if="connectionInfoStore.liveInfo()"
        :info="connectionInfoStore.liveInfo()!"
        class="stream-information__live__broadcasting-form" />
      <h4>Connection Information</h4>
      <div>
        <ListItem>
          <template #left>Host/Server</template>
          <template #right>{{ connectionInfoStore.liveInfo()?.host }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Port</template>
          <template #right>{{ connectionInfoStore.liveInfo()?.port }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Mount</template>
          <template #right>{{ connectionInfoStore.liveInfo()?.mount }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Username</template>
          <template #right>{{ connectionInfoStore.liveInfo()?.username }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Password</template>
          <template #right>
            <Button
              :loading="isPasswordLoading"
              icon="pi pi-key"
              label="Get New Password"
              @click="handleGetPassword" />
          </template>
        </ListItem>
      </div>
    </div>
    <div class="stream-information__test">
      <h2>Test Stream</h2>
      <h4>Connection Information</h4>
      <div>
        <ListItem>
          <template #left>Host/Server</template>
          <template #right>{{ connectionInfoStore.testInfo()?.host }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Port</template>
          <template #right>{{ connectionInfoStore.testInfo()?.port }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Mount</template>
          <template #right>{{ connectionInfoStore.testInfo()?.mount }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Username</template>
          <template #right>{{ connectionInfoStore.testInfo()?.username }}</template>
        </ListItem>
        <Divider style="margin: 0" />
        <ListItem>
          <template #left>Password</template>
          <template #right>{{ connectionInfoStore.testInfo()?.password }}</template>
        </ListItem>
      </div>
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
import ListItem from '@/components/data/ListItem.vue'
import { onMounted, type Ref, ref } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { Button, Dialog, Divider, InputText, useToast } from 'primevue'
import BroadcastingForm from '@/components/form/requestForms/BroadcastingForm.vue'
import usersApi from '@/api/resources/usersApi.ts'
import tryHandleUnsuccessfulResponse from '@/api/tryHandleUnsuccessfulResponse.ts'
import copyToClipboard from '@/utils/copyToClipboard.ts'

const toast = useToast()
const connectionInfoStore = useConnectionInfoStore()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})

const isPasswordLoading = ref(false)
const showPasswordDialog = ref(false)
const isCopySuccessful: Ref<boolean | null> = ref(null)
const password = ref('')

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

const handleCloseDialog = () => {
  showPasswordDialog.value = false
  isCopySuccessful.value = null
  password.value = ''
}
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.stream-information {
  &__live {
    display: flex;
    flex-direction: column;

    &__broadcasting-form {
      margin-bottom: variables.$space-l;
    }
  }

  &__test {
    display: flex;
    flex-direction: column;
  }

  &__modal-footer {
    display: flex;
    flex-direction: row;
    align-items: center;
    gap: variables.$space-m;
  }
}
</style>
