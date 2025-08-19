<template>
  <div class="test-connection-information">
    <h4>Connection Information</h4>
    <div>
      <ListItem>
        <template #left>Host/Server</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.host }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Port</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.port }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Mount</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.mount }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>User/Login</template>
        <template #right>
          <kbd>{{ connectionInfoStore.testInfo()?.username }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Password</template>
        <template #right>
          <StreamPasswordButton use-type="test" />
        </template>
      </ListItem>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Divider } from 'primevue'
import ListItem from '@/components/data/ListItem.vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import StreamPasswordButton from '@/components/controls/StreamPasswordButton.vue'

const connectionInfoStore = useConnectionInfoStore()
</script>
