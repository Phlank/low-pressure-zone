<template>
  <div class="live-connection-information">
    <h4>Connection Information</h4>
    <div>
      <ListItem>
        <template #left>Host/Server</template>
        <template #right>
          <kbd>{{ connectionInfoStore.liveInfo()?.host }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Port</template>
        <template #right>
          <kbd>{{ connectionInfoStore.liveInfo()?.port }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>Mount</template>
        <template #right>
          <kbd>{{ connectionInfoStore.liveInfo()?.mount }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem>
        <template #left>User/Login</template>
        <template #right>
          <kbd>{{ connectionInfoStore.liveInfo()?.username }}</kbd>
        </template>
      </ListItem>
      <Divider style="margin: 0" />
      <ListItem v-if="!isMobile">
        <template #left>Password</template>
        <template #right>
          <StreamPasswordButton use-type="live" />
        </template>
      </ListItem>
      <div
        v-else
        class="live-connection-information__mobile-password-button">
        <StreamPasswordButton use-type="live" />
      </div>
    </div>
  </div>
</template>

<script lang="ts" setup>
import { Divider } from 'primevue'
import ListItem from '@/components/data/ListItem.vue'
import { useConnectionInfoStore } from '@/stores/connectionInfoStore.ts'
import { inject, type Ref } from 'vue'
import StreamPasswordButton from '@/components/controls/StreamPasswordButton.vue'

const isMobile: Ref<boolean> | undefined = inject('isMobile')
const connectionInfoStore = useConnectionInfoStore()
</script>

<style lang="scss">
@use '@/assets/styles/variables';

.live-connection-information {
  &__mobile-password-button {
    display: flex;
    justify-content: center;
    margin: variables.$space-m;
  }
}
</style>
