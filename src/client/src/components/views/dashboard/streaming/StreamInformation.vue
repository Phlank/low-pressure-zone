<template>
  <div class="stream-information">
    <h2>Live Stream</h2>
    <h4>Broadcast Name</h4>
    <BroadcastingForm />
    <h4>Connection Information</h4>
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
        <Button label="Get New Password" />
      </template>
    </ListItem>
    <h2>Test Stream</h2>
    <h4>Connection Information</h4>
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
</template>

<script lang="ts" setup>
import ListItem from '@/components/data/ListItem.vue'
import { onMounted } from 'vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { Button, Divider } from 'primevue'
import BroadcastingForm from '@/components/form/requestForms/BroadcastingForm.vue'

const connectionInfoStore = useConnectionInfoStore()

onMounted(async () => {
  await connectionInfoStore.loadIfNotInitialized()
})
</script>
